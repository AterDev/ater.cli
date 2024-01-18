﻿using System.Data;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

namespace CodeGenerator.Generate;
/// <summary>
/// c#请求客户端生成
/// </summary>
public class CSHttpClientGenerate(OpenApiDocument openApi) : GenerateBase
{
    /// <summary>
    /// 
    /// </summary>
    protected OpenApiPaths PathsPairs { get; } = openApi.Paths;
    protected List<OpenApiTag> ApiTags { get; } = [.. openApi.Tags];
    public IDictionary<string, OpenApiSchema> Schemas { get; set; } = openApi.Components.Schemas;
    public OpenApiDocument OpenApi { get; set; } = openApi;

    public static string GetBaseService(string namespaceName)
    {
        var content = GetTplContent("RequestService.CsharpeBaseService.tpl");
        content = content.Replace("${Namespace}", namespaceName);
        return content;
    }

    /// <summary>
    /// 生成客户端类
    /// </summary>
    /// <returns></returns>
    public static string GetClient(List<GenFileInfo> infos, string namespaceName, string className)
    {
        var tplContent = GetTplContent("RequestService.CsharpClient.tpl");
        tplContent = tplContent.Replace("${Namespace}", namespaceName)
            .Replace("${ClassName}", className);

        var propsString = "";
        var initPropsString = "";

        infos.ForEach(info =>
        {
            propsString += @$"    public {info.ModelName}Service {info.ModelName} {{ get; init; }}" + Environment.NewLine;
            initPropsString += $"        {info.ModelName} = new {info.ModelName}Service(http);" + Environment.NewLine;
        });

        tplContent = tplContent.Replace("${Properties}", propsString)
            .Replace("${InitProperties}", initPropsString);

        return tplContent;
    }

    public static string GetGlobalUsing(string name)
    {
        var content = GetTplContent("RequestService.GlobalUsings.tpl");
        content = content + $"global using {name}.Models;" + Environment.NewLine;
        return content;
    }

    /// <summary>
    /// 项目文件
    /// </summary>
    /// <returns></returns>
    public static string GetCsprojContent()
    {
        var content = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
              </PropertyGroup>
              <ItemGroup>
                <PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />
              </ItemGroup>
            </Project>
            """;
        return content;
    }

    /// <summary>
    /// 请求服务
    /// </summary>
    /// <param name="namespaceName"></param>
    /// <returns></returns>
    public List<GenFileInfo> GetServices(string namespaceName)
    {
        List<GenFileInfo> files = [];
        List<RequestServiceFunction> functions = GetAllRequestFunctions();

        // 先以tag分组
        List<IGrouping<string?, RequestServiceFunction>> funcGroups = functions.GroupBy(f => f.Tag).ToList();
        foreach (IGrouping<string?, RequestServiceFunction>? group in funcGroups)
        {
            // 查询该标签包含的所有方法
            List<RequestServiceFunction> tagFunctions = [.. group];
            OpenApiTag? currentTag = ApiTags.Where(t => t.Name == group.Key).FirstOrDefault();
            currentTag ??= new OpenApiTag { Name = group.Key, Description = group.Key };
            RequestServiceFile serviceFile = new()
            {
                Description = currentTag.Description,
                Name = currentTag.Name!,
                Functions = tagFunctions
            };

            string content = ToRequestService(serviceFile, namespaceName);

            string fileName = currentTag.Name + "Service.cs";
            GenFileInfo file = new(fileName, content)
            {
                ModelName = currentTag.Name
            };
            files.Add(file);
        }
        return files;
    }

    /// <summary>
    /// 获取模型内容
    /// </summary>
    /// <param name="modelName"></param>
    /// <returns></returns>
    public List<GenFileInfo> GetModelFiles(string nspName)
    {
        var csGen = new CsharpModelGenerate(OpenApi);
        List<GenFileInfo> files = [];
        foreach (KeyValuePair<string, OpenApiSchema> item in Schemas)
        {
            files.Add(csGen.GenerateModelFile(item.Key, item.Value, nspName));
        }
        return files;
    }


    public static string ToRequestService(RequestServiceFile serviceFile, string namespaceName)
    {
        var functions = serviceFile.Functions;
        string functionstr = "";
        List<string> refTypes = [];
        if (functions != null)
        {
            functionstr = string.Join("\n", functions.Select(f => ToRequestFunction(f)).ToArray());

        }
        string result = $$"""
        using {{namespaceName}}.Models;
        namespace {{namespaceName}}.Services;
        /// <summary>
        /// {{serviceFile.Description}}
        /// </summary>
        public class {{serviceFile.Name}}Service(IHttpClientFactory httpClient) : BaseService(httpClient)
        {
        {{functionstr}}
        }
        """;
        return result;
    }

    public static string ToRequestFunction(RequestServiceFunction function)
    {
        function.ResponseType = string.IsNullOrWhiteSpace(function.ResponseType) ? "object" : function.ResponseType;

        // 函数名处理，去除tag前缀，然后格式化
        function.Name = function.Name.Replace(function.Tag + "_", "");
        function.Name = function.Name.ToCamelCase();
        // 处理参数
        string paramsString = "";
        string paramsComments = "";
        string dataString = "";

        if (function.Params?.Count > 0)
        {
            paramsString = string.Join(", ",
                function.Params.OrderByDescending(p => p.IsRequired)
                    .Select(p => p.IsRequired
                        ? p.Type + " " + p.Name
                        : p.Type + "? " + p.Name)
                .ToArray());
            function.Params.ForEach(p =>
            {
                //<param name="dto"></param>
                paramsComments += $"    /// <param name=\"{p.Name}\">{p.Description ?? p.Type} </param>\n";
            });
        }
        if (!string.IsNullOrEmpty(function.RequestType))
        {
            var requestType = function.RequestType == "IFile" ? "Stream" : function.RequestType;
            if (function.Params?.Count > 0)
            {
                paramsString += $", {requestType} data";
            }
            else
            {
                paramsString = $"{requestType} data";
            }

            dataString = ", data";
            paramsComments += $"    /// <param name=\"data\">{requestType}</param>\n";
        }
        // 注释生成
        string comments = $"""
             /// <summary>
             /// {function.Description ?? function.Name}
             /// </summary>
         {paramsComments}    /// <returns></returns>
         """;

        // 构造请求url
        List<string?>? paths = function.Params?.Where(p => p.InPath).Select(p => p.Name)?.ToList();
        // 需要拼接的参数,特殊处理文件上传
        List<string?>? reqParams = function.Params?.Where(p => !p.InPath && p.Type != "IForm")
            .Select(p => p.Name)?.ToList();

        if (reqParams != null)
        {
            string queryParams = "";
            queryParams = string.Join("&", reqParams.Select(p =>
            {
                return $"{p}={{{p}}}";
            }).ToArray());
            if (!string.IsNullOrEmpty(queryParams))
            {
                function.Path += "?" + queryParams;
            }
        }
        FunctionParams? file = function.Params?.Where(p => p.Type!.Equals("FormData")).FirstOrDefault();
        if (file != null)
        {
            dataString = $", {file.Name}";
        }

        var returnType = function.ResponseType == "IFile" ? "Stream?" : function.ResponseType;
        var method = function.ResponseType == "IFile"
            ? $"DownloadFileAsync(url{dataString})"
            : $"{function.Method}JsonAsync<{function.ResponseType}?>(url{dataString})";

        method = function.RequestType == "IFile" ? $"UploadFileAsync<{function.ResponseType}?>(url, new StreamContent(data))" : method;
        string res = $$"""
        {{comments}}
            public async Task<{{returnType}}> {{function.Name.ToPascalCase()}}Async({{paramsString}}) {
                var url = $"{{function.Path}}";
                return await {{method}};
            }

        """;
        return res;
    }

    /// <summary>
    /// 获取方法信息
    /// </summary>
    /// <returns></returns>
    public List<RequestServiceFunction> GetAllRequestFunctions()
    {
        List<RequestServiceFunction> functions = [];
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
                (function.RequestType, function.RequestRefType) = GetCsharpParamType(operation.Value.RequestBody?.Content.Values.FirstOrDefault()?.Schema);
                (function.ResponseType, function.ResponseRefType) = GetCsharpParamType(operation.Value.Responses.FirstOrDefault().Value
                    ?.Content.FirstOrDefault().Value
                    ?.Schema);
                function.Params = operation.Value.Parameters?.Select(p =>
                {
                    string? location = p.In?.GetDisplayName();
                    bool? inpath = location?.ToLower()?.Equals("path");
                    (string type, string? _) = GetCsharpParamType(p.Schema);
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
    /// 获取C#参数类型
    /// </summary>
    /// <param name="schema"></param>
    /// <returns></returns>
    public static (string type, string? refType) GetCsharpParamType(OpenApiSchema? schema)
    {
        if (schema == null)
        {
            return (string.Empty, string.Empty);
        }

        string? type = "object";
        string? refType = schema.Reference?.Id;
        if (schema.Reference != null)
        {
            return (schema.Reference.Id, schema.Reference.Id);
        }
        // 常规类型
        switch (schema.Type)
        {
            case "boolean":
                type = "bool";
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
                    type = "int";
                    refType = "int";
                }
                break;

            case "file":
                type = "IFile";
                break;

            case "string":
                type = schema.Format switch
                {
                    "binary" => "IFile",
                    "date-time" => "DateTimeOffset",
                    _ => "string",
                };
                break;

            case "array":
                if (schema.Items.Reference != null)
                {
                    refType = schema.Items.Reference.Id;
                    type = $"List<{refType}>";
                }
                else if (schema.Items.Type != null)
                {
                    // 基础类型处理
                    refType = schema.Items.Type;
                    refType = refType switch
                    {
                        "integer" => "int",
                        _ => refType
                    };
                    type = $"List<{refType}>";
                }
                else if (schema.Items.OneOf?.FirstOrDefault()?.Reference != null)
                {
                    refType = schema.Items.OneOf?.FirstOrDefault()!.Reference.Id;
                    type = $"List<{refType}>";
                }
                break;

            case "object":
                OpenApiSchema obj = schema.Properties.FirstOrDefault().Value;
                if (obj != null)
                {
                    if (obj.Format == "binary")
                    {
                        type = "IFile";
                    }
                }

                // TODO:object  字典
                if (schema.AdditionalProperties != null)
                {
                    var (inType, inRefType) = GetCsharpParamType(schema.AdditionalProperties);
                    refType = inRefType;
                    type = $"Dictionary<string, {inType}>";
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
}
