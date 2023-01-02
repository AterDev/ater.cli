using System.Xml.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CodeGenerator.Generate;
/// <summary>
/// 请求生成
/// </summary>
public class RequestGenerate : GenerateBase
{
    protected OpenApiPaths PathsPairs { get; }
    protected List<OpenApiTag> ApiTags { get; }
    public IDictionary<string, OpenApiSchema> Schemas { get; set; }
    public OpenApiDocument OpenApi { get; set; }

    public RequestLibType LibType { get; set; } = RequestLibType.NgHttp;

    public List<GenFileInfo>? TsModelFiles { get; set; }

    public RequestGenerate(OpenApiDocument openApi)
    {
        OpenApi = openApi;
        PathsPairs = openApi.Paths;
        Schemas = openApi.Components.Schemas;
        ApiTags = openApi.Tags.ToList();
    }

    public static string GetBaseService(RequestLibType libType)
    {
        string content = libType switch
        {
            RequestLibType.NgHttp => GetTplContent("angular.base.service.tpl"),
            RequestLibType.Axios => GetTplContent("RequestService.axios.service.tpl"),
            _ => ""
        };

        return content;
    }

    /// <summary>
    /// 获取所有请求接口解析的函数结构
    /// </summary>
    /// <returns></returns>
    public List<RequestServiceFunction> GetAllRequestFunctions()
    {
        List<RequestServiceFunction> functions = new();
        // 处理所有方法
        foreach (KeyValuePair<string, OpenApiPathItem> path in PathsPairs)
        {
            foreach (KeyValuePair<OperationType, OpenApiOperation> operation in path.Value.Operations)
            {
                RequestServiceFunction function = new()
                {
                    Description = operation.Value.Summary,
                    Method = operation.Key.ToString(),
                    Name = operation.Value.OperationId,
                    Path = path.Key,
                    Tag = operation.Value.Tags.FirstOrDefault()?.Name,
                };
                (function.RequestType, function.RequestRefType) = GetParamType(operation.Value.RequestBody?.Content.Values.FirstOrDefault()?.Schema);
                (function.ResponseType, function.ResponseRefType) = GetParamType(operation.Value.Responses.FirstOrDefault().Value
                    ?.Content.FirstOrDefault().Value
                    ?.Schema);
                function.Params = operation.Value.Parameters?.Select(p =>
                {
                    string? location = p.In?.GetDisplayName();
                    bool? inpath = location?.ToLower()?.Equals("path");
                    (string type, string _) = GetParamType(p.Schema);
                    return new FunctionParams
                    {
                        Description = p.Description,
                        Name = p.Name,
                        InPath = inpath ?? false,
                        IsRequired = p.Required,
                        Type = type
                    };
                }).ToList();

                functions.Add(function);
            }
        }
        return functions;
    }

    /// <summary>
    /// 根据tag生成多个请求服务文件
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    public List<GenFileInfo> GetServices(IList<OpenApiTag> tags)
    {
        if (TsModelFiles == null)
        {
            GetTSInterfaces();
        }
        List<GenFileInfo> files = new();
        List<RequestServiceFunction> functions = GetAllRequestFunctions();

        // 先以tag分组
        List<IGrouping<string?, RequestServiceFunction>> funcGroups = functions.GroupBy(f => f.Tag).ToList();
        foreach (IGrouping<string?, RequestServiceFunction>? group in funcGroups)
        {
            // 查询该标签包含的所有方法
            List<RequestServiceFunction> tagFunctions = group.ToList();
            OpenApiTag? currentTag = tags.Where(t => t.Name == group.Key).FirstOrDefault();
            currentTag ??= new OpenApiTag { Name = group.Key, Description = group.Key };
            RequestServiceFile serviceFile = new()
            {
                Description = currentTag.Description,
                Name = currentTag.Name!,
                Functions = tagFunctions
            };

            string content = LibType switch
            {
                RequestLibType.NgHttp => ToNgRequestService(serviceFile),
                RequestLibType.Axios => ToAxiosRequestService(serviceFile),
                _ => ""
            };

            string fileName = currentTag.Name?.ToHyphen() + ".service.ts";
            GenFileInfo file = new(content)
            {
                Name = fileName,
            };
            files.Add(file);
        }
        return files;
    }

    /// <summary>
    /// ts interface files
    /// </summary>
    /// <returns></returns>
    public List<GenFileInfo> GetTSInterfaces()
    {
        TSModelGenerate tsGen = new(OpenApi);
        List<GenFileInfo> files = new();
        foreach (KeyValuePair<string, OpenApiSchema> item in Schemas)
        {
            files.Add(tsGen.GenerateInterfaceFile(item.Key, item.Value));
        }
        TsModelFiles = files;
        return files;
    }

    public static (string type, string? refType) GetParamType(OpenApiSchema? schema)
    {
        if (schema == null)
        {
            return (string.Empty, string.Empty);
        }

        string? type = "any";
        string? refType = schema.Reference?.Id;
        if (schema.Reference != null)
        {
            return (schema.Reference.Id, schema.Reference.Id);
        }
        // 常规类型
        switch (schema.Type)
        {
            case "boolean":
                type = "boolean";
                break;
            case "integer":
                // 看是否为enum
                if (schema.Enum.Count > 0)
                {
                    if (schema.Reference != null)
                    {
                        type = schema.Reference.Id;
                        refType = schema.Reference.Id;
                    }
                }
                else
                {
                    type = "number";
                }
                break;
            case "file":
                type = "FormData";
                break;
            case "string":
                type = schema.Format switch
                {
                    "binary" => "FormData",
                    "date-time" => "string",
                    _ => "string",
                };
                break;

            case "array":
                if (schema.Items.Reference != null)
                {
                    refType = schema.Items.Reference.Id;
                    type = refType + "[]";
                }
                else if (schema.Items.Type != null)
                {
                    // 基础类型?
                    refType = schema.Items.Type;
                    type = refType + "[]";
                }
                else if (schema.Items.OneOf?.FirstOrDefault()?.Reference != null)
                {
                    refType = schema.Items.OneOf?.FirstOrDefault()!.Reference.Id;
                    type = refType + "[]";
                }
                break;
            case "object":
                OpenApiSchema obj = schema.Properties.FirstOrDefault().Value;
                if (obj != null)
                {
                    if (obj.Format == "binary")
                    {
                        type = "FormData";
                    }
                }
                break;
            default:
                break;
        }
        // 引用对象
        if (schema.OneOf.Count > 0)
        {
            // 获取引用对象名称
            type = schema.OneOf.First()?.Reference.Id ?? type;
            refType = schema.OneOf.First()?.Reference.Id;
        }
        return (type, refType);
    }

    public string ToAxiosRequestService(RequestServiceFile serviceFile)
    {
        string tplContent = GetTplContent("RequestService.service.ts");
        string functionString = "";
        List<RequestServiceFunction>? functions = serviceFile.Functions;
        // import引用的models
        string importModels = "";
        if (functions != null)
        {
            functionString = string.Join("\n",
                functions.Select(ToAxiosFunction).ToArray());
            List<string> refTypes = GetRefTyeps(functions);
            refTypes.ForEach(t =>
            {
                string? dirName = TsModelFiles?.Where(f => f.ModelName == t).Select(f => f.Path).FirstOrDefault();
                importModels += $"import {{ {t} }} from '../models/{dirName?.ToHyphen()}/{t.ToHyphen()}.model';{Environment.NewLine}";
            });
        }
        tplContent = tplContent.Replace("[@Import]", importModels)
            .Replace("[@ServiceName]", serviceFile.Name)
            .Replace("[@Functions]", functionString);
        return tplContent;
    }

    public string ToNgRequestService(RequestServiceFile serviceFile)
    {
        var functions = serviceFile.Functions;
        string functionstr = "";
        // import引用的models
        string importModels = "";
        List<string> refTypes = new();
        if (functions != null)
        {
            functionstr = string.Join("\n", functions.Select(f => f.ToFunction()).ToArray());
            string[] baseTypes = new string[] { "string", "string[]", "number", "number[]", "boolean" };
            // 获取请求和响应的类型，以便导入
            List<string?> requestRefs = functions
                .Where(f => !string.IsNullOrEmpty(f.RequestRefType)
                    && !baseTypes.Contains(f.RequestRefType))
                .Select(f => f.RequestRefType).ToList();
            List<string?> responseRefs = functions
                .Where(f => !string.IsNullOrEmpty(f.ResponseRefType)
                    && !baseTypes.Contains(f.ResponseRefType))
                .Select(f => f.ResponseRefType).ToList();

            // 参数中的类型
            List<string?> paramsRefs = functions.SelectMany(f => f.Params!)
                .Where(p => !baseTypes.Contains(p.Type))
                .Select(p => p.Type)
                .ToList();
            if (requestRefs != null)
            {
                refTypes.AddRange(requestRefs!);
            }

            if (responseRefs != null)
            {
                refTypes.AddRange(responseRefs!);
            }

            if (paramsRefs != null)
            {
                refTypes.AddRange(paramsRefs!);
            }

            refTypes = refTypes.GroupBy(t => t)
                .Select(g => g.FirstOrDefault()!)
                .ToList();

            refTypes.ForEach(t =>
            {
                if (Config.EnumModels.Contains(t))
                {
                    importModels += $"import {{ {t} }} from '../models/enum/{t.ToHyphen()}.model';{Environment.NewLine}";
                }
                else
                {
                    string? dirName = TsModelFiles?.Where(f => f.ModelName == t).Select(f => f.Path).FirstOrDefault();
                    importModels += $"import {{ {t} }} from '../models/{dirName?.ToHyphen()}/{t.ToHyphen()}.model';{Environment.NewLine}";
                }

            });
        }
        string result = $@"import {{ Injectable }} from '@angular/core';
import {{ BaseService }} from './base.service';
import {{ Observable }} from 'rxjs';

        {importModels}
/**
 * {serviceFile.Description}
 */
@Injectable({{ providedIn: 'root' }})
export class {serviceFile.Name}Service extends BaseService {{
{functionstr}
}}
";
        return result;
    }

    /// <summary>
    /// axios函数格式
    /// </summary>
    /// <param name="function"></param>
    /// <returns></returns>
    protected string ToAxiosFunction(RequestServiceFunction function)
    {
        string Name = function.Name;
        List<FunctionParams>? Params = function.Params;
        string RequestType = function.RequestType;
        string? ResponseType = function.ResponseType;
        string Path = function.Path;

        // 函数名处理，去除tag前缀，然后格式化
        Name = Name.Replace(function.Tag + "_", "");
        Name = Name.ToCamelCase();
        // 处理参数
        string paramsString = "";
        string paramsComments = "";
        string dataString = "";
        if (Params?.Count > 0)
        {
            paramsString = string.Join(", ",
                Params.OrderBy(p => p.IsRequired)
                    .Select(p => p.Name + ": " + p.Type)
                .ToArray());
            Params.ForEach(p =>
            {
                paramsComments += $"   * @param {p.Name} {p.Description ?? p.Type}\n";
            });
        }
        if (!string.IsNullOrEmpty(RequestType))
        {
            if (Params?.Count > 0)
            {
                paramsString += $", data: {RequestType}";
            }
            else
            {
                paramsString = $"data: {RequestType}";
            }

            dataString = ", data";
            paramsComments += $"   * @param data {RequestType}\n";
        }
        // 添加extOptions
        if (!string.IsNullOrWhiteSpace(paramsComments))
        {
            paramsString += ", ";
        }
        paramsString += "extOptions?: ExtOptions";
        // 注释生成
        string comments = $@"  /**
   * {function.Description ?? Name}
{paramsComments}   */";

        // 构造请求url
        List<string?>? paths = Params?.Where(p => p.InPath).Select(p => p.Name)?.ToList();
        paths?.ForEach(p =>
            {
                string origin = $"{{{p}}}";
                Path = Path.Replace(origin, "$" + origin);
            });
        // 需要拼接的参数,特殊处理文件上传
        List<string?>? reqParams = Params?.Where(p => !p.InPath && p.Type != "FormData")
            .Select(p => p.Name)?.ToList();
        if (reqParams != null)
        {
            string queryParams = "";
            queryParams = string.Join("&", reqParams.Select(p => { return $"{p}=${{{p}}}"; }).ToArray());
            if (!string.IsNullOrEmpty(queryParams))
            {
                Path += "?" + queryParams;
            }
        }
        // 上传文件时的名称
        FunctionParams? file = Params?.Where(p => p.Type!.Equals("FormData")).FirstOrDefault();
        if (file != null)
        {
            dataString = $", {file.Name}";
        }

        // 默认添加ext
        if (string.IsNullOrEmpty(dataString))
        {
            dataString = ", null, extOptions";
        }
        else
        {
            dataString += ", extOptions";
        }
        string functionString = @$"{comments}
  {Name}({paramsString}): Promise<{ResponseType}> {{
    const url = `{Path}`;
    return this.request<{ResponseType}>('{function.Method.ToLower()}', url{dataString});
  }}
";
        return functionString;
    }

    /// <summary>
    /// 获取要导入的依赖
    /// </summary>
    /// <param name="functions"></param>
    /// <returns></returns>
    protected List<string> GetRefTyeps(List<RequestServiceFunction> functions)
    {
        List<string> refTypes = new();

        string[] baseTypes = new string[] { "string", "string[]", "number", "number[]", "boolean" };
        // 获取请求和响应的类型，以便导入
        List<string?> requestRefs = functions
                .Where(f => !string.IsNullOrEmpty(f.RequestRefType)
                    && !baseTypes.Contains(f.RequestRefType))
                .Select(f => f.RequestRefType).ToList();
        List<string?> responseRefs = functions
                .Where(f => !string.IsNullOrEmpty(f.ResponseRefType)
                    && !baseTypes.Contains(f.ResponseRefType))
                .Select(f => f.ResponseRefType).ToList();

        // 参数中的类型
        List<string?> paramsRefs = functions.SelectMany(f => f.Params!)
                .Where(p => !baseTypes.Contains(p.Type))
                .Select(p => p.Type)
                .ToList();
        if (requestRefs != null)
        {
            refTypes.AddRange(requestRefs!);
        }

        if (responseRefs != null)
        {
            refTypes.AddRange(responseRefs!);
        }

        if (paramsRefs != null)
        {
            refTypes.AddRange(paramsRefs!);
        }

        refTypes = refTypes.GroupBy(t => t)
            .Select(g => g.FirstOrDefault()!)
            .ToList();
        return refTypes;
    }
}
public enum RequestLibType
{
    NgHttp,
    Axios
}