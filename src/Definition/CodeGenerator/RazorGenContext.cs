using RazorEngineCore;

namespace CodeGenerator;

/// <summary>
/// ��������������
/// </summary>
public class RazorGenContext
{
    public IRazorEngine RazorEngine { get; set; } = new RazorEngine();
    public string GenManager(string templateContent, ManagerViewModel model)
    {
        return GenCode(templateContent, model);
    }

    /// <summary>
    /// �ض���������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="templateContent"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public string GenCode<T>(string templateContent, T model)
    {
        IRazorEngineCompiledTemplate<RazorEngineTemplateBase<T>> template = RazorEngine.Compile<RazorEngineTemplateBase<T>>(templateContent);
        string result = template.Run(instance =>
        {
            instance.Model = model;
        });
        return result;
    }

    /// <summary>
    /// ����ģ���滻
    /// </summary>
    /// <param name="templateContent"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public string GenTemplate(string templateContent, List<Variable> model)
    {
        // model to dictionary 
        var dictionary = model.ToDictionary(v => v.Key, v => v.Value);

        var template = RazorEngine.Compile<RazorEngineTemplateBase<Dictionary<string, string>>>(templateContent);
        string result = template.Run(instance =>
        {
            instance.Model = dictionary;
        });
        return result;
    }

    public string GenTemplate(string templateContent, ActionRunModel model)
    {
        templateContent = "@using Ater.Web.Core.Utils;" + Environment.NewLine + templateContent;
        var dictionary = model.Variables.ToDictionary(v => v.Key, v => v.Value);

        var template = RazorEngine.Compile<CustomTemplate>(templateContent, builder =>
        {
            builder.AddAssemblyReferenceByName("System.Collections");
            builder.AddAssemblyReferenceByName("System");
            builder.AddAssemblyReferenceByName("Ater.Web.Core");
            builder.AddAssemblyReferenceByName("Entity");
        });

        string result = template.Run(instance =>
        {
            instance.Variables = dictionary;
            instance.ModelName = model.ModelName;
            instance.Namespace = model.Namespace;
            instance.PropertyInfos = model.PropertyInfos;
            instance.NewLine = Environment.NewLine;
        });
        return result;
    }
}

public class CustomTemplate : RazorEngineTemplateBase
{
    public Dictionary<string, string> Variables { get; set; } = [];
    /// <summary>
    /// ģ������
    /// </summary>
    public string? ModelName { get; set; }
    /// <summary>
    /// �����ռ�
    /// </summary>
    public string? Namespace { get; set; }

    /// <summary>
    /// ��������
    /// </summary>
    public string? Description { get; set; }

    public string NewLine { get; set; } = Environment.NewLine;

    public List<PropertyInfo> PropertyInfos { get; set; } = [];
}

public class ActionRunModel
{
    public List<Variable> Variables { get; set; } = [];
    /// <summary>
    /// ģ������
    /// </summary>
    public string? ModelName { get; set; }
    /// <summary>
    /// �����ռ�
    /// </summary>
    public string? Namespace { get; set; }

    /// <summary>
    /// ��������
    /// </summary>
    public string? Description { get; set; }

    public List<PropertyInfo> PropertyInfos { get; set; } = [];
}