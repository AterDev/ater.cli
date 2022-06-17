﻿using Microsoft.OpenApi.Models;

namespace CodeGenerator.Generate;
public class DocGenerate : GenerateBase
{
    public IDictionary<string, OpenApiSchema> Schemas { get; set; }
    public List<OpenApiTag>? ApiTags { get; set; }
    public DocGenerate(IDictionary<string, OpenApiSchema> schemas)
    {
        Schemas = schemas;
    }
    public void SetTags(List<OpenApiTag> apiTags)
    {
        ApiTags = apiTags;
    }


    public string GetMarkdownContent()
    {
        var docContent = "";
        var itemContent = "";
        var tocContent = "- [目录](#目录)" + Environment.NewLine;

        Schemas = Schemas.OrderBy(s => s.Key)
            .ToDictionary(s => s.Key, s => s.Value);

        foreach (var schema in Schemas)
        {
            var description = schema.Value.AllOf.LastOrDefault()?.Description
                ??schema.Value.Description;

            description = description?.Replace("\n", " ") ?? "";
            // 构建目录

            var des = string.IsNullOrEmpty(description) ? "" : "-"+description;
            des = des.Replace(" ", "-")
                .Replace(" = ", "=")
                .Replace(",", "");

            if (!string.IsNullOrEmpty(description)) description = $"({description})".Replace(" = ", "=");

            var toc = $"\t- [{schema.Key} {description}](#{schema.Key.ToLower()}{des})" + Environment.NewLine;

            tocContent += toc;

            var header = $"### [{schema.Key}](#{schema.Key}) {description}" + Environment.NewLine;

            var props = TSModelGenerate.GetTsProperties(schema.Value);

            var row = "|字段名|类型|必须|说明|" + Environment.NewLine;
            row += "|-|-|-|-|" + Environment.NewLine;
            props.ForEach(p =>
            {
                row += FormatProperty(p);
            });
            itemContent += header + row + Environment.NewLine;
        }
        docContent = tocContent + itemContent;
        return docContent;
    }



    public static string FormatProperty(TsProperty property)
    {
        var comments = property.Comments?.Replace("*","")
            .Replace("/","")
            .Replace("\r\n",Environment.NewLine)
            .Replace("\n\r",Environment.NewLine)
            .Replace("\r",Environment.NewLine)
            .Replace(Environment.NewLine+"*","")
            .Replace(Environment.NewLine,"")
            .Replace("\n",";")
            .Replace(" ","")
            .Trim(new char[] { ';' });

        if (string.IsNullOrEmpty(comments)) { comments = "-"; }

        var type = property.Type?.Replace(" | null","");
        var isMust = property.IsNullable==false?"false":"true";

        return $"|{property.Name}" +
            $"|{type}|{isMust}|{comments}|" +
            Environment.NewLine;
    }
}
