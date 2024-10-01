﻿using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
using Share.Models;

namespace Share.Services;
/// <summary>
/// 代码解析服务
/// </summary>
public class CodeAnalysisService(IProjectContext projectContext)
{
    private readonly IProjectContext _projectContext = projectContext;

    /// <summary>
    /// get entity file info
    /// </summary>
    /// <param name="filePaths"></param>
    /// <returns></returns>
    public List<EntityFile> GetEntityFiles(List<string> filePaths)
    {
        var entityFiles = new ConcurrentBag<EntityFile>();
        Parallel.ForEach(filePaths, path =>
        {
            string content = File.ReadAllText(path);
            var compilation = new CompilationHelper(_projectContext.EntityPath!);
            compilation.LoadContent(content);
            if (compilation.IsEntityClass())
            {
                var comment = RegexSource.SummaryCommentRegex()
                    .Match(content)?.Groups[1]?.Value.Trim();
                comment = comment?.Replace("/", "").Trim();

                var entityFile = new EntityFile
                {
                    Name = Path.GetFileName(path),
                    FullName = path,
                    Content = content,
                    Comment = comment,
                };
                var moduleAttribution = compilation.GetClassAttribution("Module");
                if (moduleAttribution != null && moduleAttribution.Count != 0)
                {
                    var argument = moduleAttribution.Last().ArgumentList?.Arguments.FirstOrDefault();
                    if (argument != null)
                    {
                        entityFile.ModuleName = compilation.GetArgumentValue(argument);
                    }
                }
                entityFiles.Add(entityFile);
            }
        });
        return [.. entityFiles];
    }

    /// <summary>
    /// 分析实体信息
    /// </summary>
    /// <param name="entityFiles"></param>
    /// <returns></returns>
    public static List<EntityInfo> GetEntityInfos(List<string> entityFiles)
    {
        var entityInfos = new ConcurrentBag<EntityInfo>();
        Parallel.ForEach(entityFiles, entityFile =>
        {
            var parse = new EntityParseHelper(entityFile);
            var entityInfo = parse.ParseEntityAsync().Result;

            entityInfos.Add(entityInfo);
        });
        return [.. entityInfos];
    }

    public EntityFile? GetEntityFile(string filePath)
    {
        return GetEntityFiles([filePath]).FirstOrDefault();
    }

    /// <summary>
    /// get entity files path
    /// </summary>
    /// <param name="entityAssemblyPath"></param>
    /// <returns></returns>
    public static List<string> GetEntityFilePaths(string entityAssemblyPath)
    {
        return Directory.GetFiles(entityAssemblyPath, "*.cs", SearchOption.AllDirectories)
            .Where(f => !(f.EndsWith(".g.cs")
                    || f.EndsWith(".AssemblyAttributes.cs")
                    || f.EndsWith(".AssemblyInfo.cs")
                    || f.EndsWith("GlobalUsings.cs")
                    || f.EndsWith("EntityBase.cs")
                    || f.EndsWith("Modules.cs"))
                    ).ToList();
    }

}