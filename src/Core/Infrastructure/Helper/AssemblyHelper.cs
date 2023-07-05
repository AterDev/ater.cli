﻿using System.Xml.Linq;
using NuGet.Versioning;

namespace Core.Infrastructure.Helper;

/// <summary>
/// 项目帮助类
/// </summary>
public class AssemblyHelper
{
    /// <summary>
    /// 搜索项目文件.csproj,直到根目录
    /// </summary>
    /// <param name="dir">起始目录</param>
    /// <param name="root">根目录</param>
    /// <returns></returns>
    public static FileInfo? FindProjectFile(DirectoryInfo dir, DirectoryInfo? root = null)
    {
        try
        {
            FileInfo? file = dir.GetFiles("*.csproj")?.FirstOrDefault();
            return root == null ? file : file == null && dir != root ? FindProjectFile(dir.Parent!, root) : file;
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine("❌ can't find dir:" + dir.FullName);
            return null;
        }
    }

    /// <summary>
    /// 在项目中寻找文件
    /// </summary>
    /// <param name="projectFilePath"></param>
    /// <param name="searchFileName"></param>
    /// <returns>the search file path,return null if not found </returns>
    public static string? FindFileInProject(string projectFilePath, string searchFileName)
    {
        DirectoryInfo dir = new(Path.GetDirectoryName(projectFilePath)!);
        string[] files = Directory.GetFiles(dir.FullName, searchFileName, SearchOption.AllDirectories);
        return files.Any() ? files[0] : default;
    }

    /// <summary>
    /// 解析项目文件xml 获取名称,没有自定义则取文件名
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static string GetAssemblyName(FileInfo file)
    {
        XElement xml = XElement.Load(file.FullName);
        XElement? node = xml.Descendants("PropertyGroup")
            .SelectMany(pg => pg.Elements())
            .Where(el => el.Name.LocalName.Equals("AssemblyName"))
            .FirstOrDefault();
        // 默认名称
        string name = Path.GetFileNameWithoutExtension(file.Name);
        if (node != null)
        {
            if (!node.Value.Contains("$(MSBuildProjectName)"))
            {
                name = node.Value;
            }
        }
        return name;
    }

    /// <summary>
    /// 获取项目类型
    /// </summary>
    /// <param name="file"></param>
    /// <returns>oneOf: null/web/console</returns>
    public static string? GetProjectType(FileInfo file)
    {
        XElement xml = XElement.Load(file.FullName);
        var sdk = xml.Attribute("Sdk")?.Value;
        // TODO:仅判断是否为web
        return sdk == null ? null :
            sdk.EndsWith("Sdk.Web")
            ? "web"
            : "console";
    }

    public static string? GetAssemblyName(DirectoryInfo dir)
    {
        FileInfo? file = FindProjectFile(dir);
        return file == null ? null : GetAssemblyName(file);
    }

    /// <summary>
    /// 获取命名空间名称， 不支持MSBuildProjectName表达式
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static string? GetNamespaceName(DirectoryInfo dir)
    {
        FileInfo? file = FindProjectFile(dir);
        if (file == null)
        {
            return null;
        }

        XElement xml = XElement.Load(file.FullName);
        XElement? node = xml.Descendants("PropertyGroup")
            .SelectMany(pg => pg.Elements())
            .Where(el => el.Name.LocalName.Equals("RootNamespace"))
            .FirstOrDefault();
        // 默认名称
        string name = Path.GetFileNameWithoutExtension(file.Name);
        if (node != null)
        {
            if (!node.Value.Contains("MSBuildProjectName"))
            {
                name = node.Value;
            }
        }
        return name;
    }

    /// <summary>
    /// 获取解决方案文件，从当前目录向根目录搜索
    /// </summary>
    /// <param name="dir">当前目录</param>
    /// <param name="searchPattern"></param>
    /// <param name="root">要目录</param>
    /// <returns></returns>
    public static FileInfo? GetSlnFile(DirectoryInfo dir, string searchPattern, DirectoryInfo? root = null)
    {
        try
        {
            FileInfo? file = dir.GetFiles("*.sln")?.FirstOrDefault();
            return root == null ? file
                : file == null && dir != root ? GetSlnFile(dir.Parent!, searchPattern, root) : file;
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <summary>
    /// 获取git根目录
    /// </summary>
    /// <param name="dir">搜索目录，从该目录向上递归搜索</param>
    /// <returns></returns>
    public static DirectoryInfo? GetGitRoot(DirectoryInfo dir)
    {
        try
        {
            DirectoryInfo? directory = dir.GetDirectories(".git").FirstOrDefault();
            return directory != null
                ? directory.Parent
                : directory == null && dir.Root != dir && dir.Parent != null ? GetGitRoot(dir.Parent) : default;
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <summary>
    /// 获取版本
    /// </summary>
    /// <returns></returns>
    public static string GetVersion()
    {
        return Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
    }

    /// <summary>
    /// 获取当前项目下的 xml 注释中的members
    /// </summary>
    /// <returns></returns>
    public static List<XmlCommentMember>? GetXmlMembers(DirectoryInfo dir)
    {
        FileInfo? projectFile = dir.GetFiles("*.csproj")?.FirstOrDefault();
        if (projectFile != null)
        {
            string assemblyName = GetAssemblyName(projectFile);
            FileInfo? xmlFile = dir.GetFiles($"{assemblyName}.xml", SearchOption.AllDirectories).FirstOrDefault();
            if (xmlFile != null)
            {
                XElement xml = XElement.Load(xmlFile.FullName);
                List<XmlCommentMember> members = xml.Descendants("member")
                    .Select(s => new XmlCommentMember
                    {
                        FullName = s.Attribute("name")?.Value ?? "",
                        Summary = s.Element("summary")?.Value

                    }).ToList();
                return members;
            }
        }
        return null;
    }


    /// <summary>
    /// 判断是否需要更新
    /// </summary>
    /// <param name="minVersionStr">最小版本号</param>
    /// <returns></returns>
    public static bool NeedUpdate(string minVersionStr)
    {
        var minVersion = NuGetVersion.Parse(minVersionStr);
        var oldVerion = NuGetVersion.Parse(Config.Version);
        var currentVersion = NuGetVersion.Parse(GetVersion());

        Console.WriteLine($"project version:{oldVerion}; studio version:{currentVersion}");
        return VersionComparer.Compare(oldVerion, minVersion, VersionComparison.Version) >= 0
            && VersionComparer.Compare(oldVerion, currentVersion, VersionComparison.Version) < 0;
    }

    public class XmlCommentMember
    {
        public string FullName { get; set; } = string.Empty;
        public string? Summary { get; set; }
    }

    /// <summary>
    /// get csproject targetFramework 
    /// </summary>
    /// <param name="projectPath"></param>
    /// <returns></returns>
    public static string? GetTargetFramework(string projectPath)
    {
        XElement xml = XElement.Load(projectPath);
        var targetFramework = xml.Descendants("TargetFramework").FirstOrDefault();
        return targetFramework?.Value;
    }

    /// <summary>
    /// 生成文件
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="fileName"></param>
    /// <param name="content"></param>
    /// <param name="cover"></param>
    /// <returns></returns>
    public static async Task GenerateFileAsync(string dir, string fileName, string content, bool cover = false)
    {
        if (!Directory.Exists(dir))
        {
            _ = Directory.CreateDirectory(dir);
        }
        string filePath = Path.Combine(dir, fileName);
        if (!File.Exists(filePath) || cover)
        {
            await File.WriteAllTextAsync(filePath, content);
            Console.WriteLine(@$"  🆕 generate file {fileName}.");
        }
        else
        {
            Console.WriteLine($"  🦘 Skip exist file: {fileName}.");
        }
    }
}
