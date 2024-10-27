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

    public string GenTemplate(string templateContent, TempModel model)
    {
        var template = RazorEngine.Compile<RazorEngineTemplateBase<TempModel>>(templateContent);
        string result = template.Run(instance =>
        {
            instance.Model = model;
        });
        return result;
    }
}
public class TempModel
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