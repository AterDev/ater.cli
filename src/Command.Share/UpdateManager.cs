﻿using System.Text.Json;

using NuGet.Versioning;

namespace Command.Share;
/// <summary>
/// 更新管理
/// </summary>
public class UpdateManager
{
    /// <summary>
    /// 版本更新
    /// </summary>
    /// <param name="solutionFilePath"></param>
    /// <param name="currentVersion"></param>11
    public static bool UpdateInfrastructure(string solutionFilePath, string currentVersion, out string newVersion)
    {
        var solutionPath = Path.GetDirectoryName(solutionFilePath)!;
        newVersion = currentVersion;
        var version = NuGetVersion.Parse(currentVersion);
        // 7.0->7.1
        if (version == NuGetVersion.Parse("7.0.0"))
        {
            UpdateExtensionAsync7(solutionPath).Wait();
            UpdateConst7(solutionPath);
            UpdateCustomizeAttribution7(solutionPath);
            var configFilePath = Path.Combine(solutionPath, Config.ConfigFileName);
            if (File.Exists(configFilePath))
            {
                var config = JsonSerializer.Deserialize<ConfigOptions>(File.ReadAllText(configFilePath));
                if (config != null)
                {
                    config.Version = "7.1.0";
                    File.WriteAllText(configFilePath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
                }
                newVersion = "7.1.0";
                return true;
            }
        }

        if (version == NuGetVersion.Parse("7.1.0"))
        {
            var res = UpdateTo8Async(solutionPath).Result;
            if (res)
            {
                newVersion = "8.0.0";
                return true;
            }
        }
        return false;
    }

    #region 7.0->7.1更新

    /// <summary>
    /// 更新扩展方法
    /// </summary>
    /// <param name="solutionPath"></param>
    /// <returns></returns>
    private static async Task UpdateExtensionAsync7(string solutionPath)
    {
        var extensionPath = Path.Combine(solutionPath, Config.EntityPath, "Utils", "Extensions.cs");
        if (File.Exists(extensionPath))
        {
            CompilationHelper compilation = new(Path.Combine(solutionPath, Config.EntityPath));
            compilation.AddSyntaxTree(File.ReadAllText(extensionPath));
            if (!compilation.MethodExist("public static IQueryable<TSource> WhereNotNull<TSource>(this IQueryable<TSource> source, object? field, Expression<Func<TSource, bool>> expression)"))
            {
                string whereNotNullString = """
                    public static IQueryable<TSource> WhereNotNull<TSource>(this IQueryable<TSource> source, object? field, Expression<Func<TSource, bool>> expression)
                    {
                        return field != null ? source.Where(expression) : source;
                    }
                """;
                compilation.InsertClassMethod(whereNotNullString);
                string newClassContent = compilation.SyntaxRoot!.ToString();
                await CommandBase.GenerateFileAsync(
                    Path.Combine(extensionPath, ".."),
                    "Extensions.cs",
                    newClassContent,
                    true);

                await Console.Out.WriteLineAsync("👉 add [WhereNotNull] method to Extension.cs!");
            }
        }
        else
        {
            Console.WriteLine($"⚠️ can't find {extensionPath}");
        }
    }

    /// <summary>
    /// 更新常量文件
    /// </summary>
    private static void UpdateConst7(string solutionPath)
    {
        var applicationPath = Path.Combine(solutionPath, Config.ApplicationPath);
        Console.WriteLine("⬆️ Update app const.");
        string errorMsgPath = Path.Combine(applicationPath, "Const", "ErrorMsg.cs");
        string appConstPath = Path.Combine(applicationPath, "Const", "AppConst.cs");
        if (!File.Exists(errorMsgPath))
        {
            if (!Directory.Exists(Path.Combine(applicationPath, "Const")))
            {
                _ = Directory.CreateDirectory(Path.Combine(applicationPath, "Const"));
            }
            if (!File.Exists(errorMsgPath))
            {
                File.WriteAllText(errorMsgPath, """
                    namespace Application.Const;
                    /// <summary>
                    /// 错误信息
                    /// </summary>
                    public static class ErrorMsg
                    {
                        /// <summary>
                        /// 未找到该用户
                        /// </summary>
                        public const string NotFoundUser = "未找到该用户!";
                        /// <summary>
                        /// 未找到的资源
                        /// </summary>
                        public const string NotFoundResource = "未找到的资源!";
                    }
                    """, Encoding.UTF8);
            }
            if (!File.Exists(appConstPath))
            {
                File.WriteAllText(appConstPath, """
                    namespace Application.Const;
                    /// <summary>
                    /// 应用程序常量
                    /// </summary>
                    public static class AppConst
                    {
                        public const string DefaultStateName = "statestore";
                        public const string DefaultPubSubName = "pubsub";

                        /// <summary>
                        /// 管理员policy
                        /// </summary>
                        public const string AdminUser = "AdminUser";
                        /// <summary>
                        /// 普通用户policy
                        /// </summary>
                        public const string User = "User";

                        /// <summary>
                        /// 版本
                        /// </summary>
                        public const string Version = "Version";
                    }
                    """, Encoding.UTF8);
            }

            Console.WriteLine("🔔 App Const move to Application, please remove it from Core!");
        }
    }

    /// <summary>
    /// 自定义特性文件
    /// </summary>
    /// <param name="solutionPath"></param>
    private static void UpdateCustomizeAttribution7(string solutionPath)
    {
        Console.WriteLine("⬆️ Update customize attributes.");
        var path = Path.Combine(solutionPath, Config.EntityPath, "CustomizeAttribute.cs");
        var oldFile = Path.Combine(solutionPath, Config.EntityPath, "NgPageAttribute.cs");

        if (!File.Exists(path))
        {
            if (File.Exists(oldFile))
            {
                File.Delete(oldFile);
            }
            File.WriteAllTextAsync(path, """
            namespace Core;

            /// <summary>
            /// 模块标记
            /// </summary>
            [AttributeUsage(AttributeTargets.Class)]
            public class ModuleAttribute : Attribute
            {
                /// <summary>
                /// 模块名称，区分大小写
                /// </summary>
                public string Name { get; init; }

                public ModuleAttribute(string name)
                {
                    Name = name;
                }
            }
            """, Encoding.UTF8);
        }
    }

    #endregion

    #region 7.1更新到8.0
    /// <summary>
    /// 升级到8.0
    /// </summary>
    /// <param name="solutionPath"></param>
    public static async Task<bool> UpdateTo8Async(string solutionPath)
    {
        var solutionFilePath = Directory.GetFiles(solutionPath, "*.sln", SearchOption.TopDirectoryOnly).FirstOrDefault();

        var aterCoreName = "Ater.Web.Core";
        var aterAbstracture = "Ater.Web.Abstracture";

        if (solutionFilePath == null)
        {
            Console.WriteLine("⚠️ can't find sln file");
            return false;
        }
        try
        {
            var solution = new SolutionHelper(solutionFilePath);

            // 添加Infrastructure
            var studioPath = AssemblyHelper.GetStudioPath();
            var fromDir = Path.Combine(studioPath, "Infrastructure");
            var destDir = Path.Combine(solutionPath, "src", "Infrastructure");
            // copy Infrastructure
            if (Directory.Exists(fromDir))
            {
                if (Directory.Exists(destDir))
                {
                    // TODO: 备份？
                    Directory.Delete(destDir, true);
                }
                Directory.CreateDirectory(destDir);
                IOHelper.CopyDirectory(fromDir, destDir);
                // add to solution
                await solution.AddExistProjectAsync(Path.Combine(destDir, aterCoreName, $"{aterCoreName}.csproj"));
                await solution.AddExistProjectAsync(Path.Combine(destDir, aterAbstracture, $"{aterAbstracture}.csproj"));
            }
            else
            {
                Console.WriteLine($"⚠️ can't find {fromDir}");
            }


            // 迁移原Core到新Entity
            var coreProjectFilePath = Directory.GetFiles(Path.Combine(solutionPath, Config.EntityPath), "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();

            if (coreProjectFilePath == null)
            {
                Console.WriteLine($"Orignal Core project not found:{0}", coreProjectFilePath);
                return false;
            }
            solution.RemoveProject(Path.GetFileNameWithoutExtension(coreProjectFilePath));

            var entitiesDir = Path.Combine(solutionPath, Config.EntityPath, "Entities");
            destDir = Path.Combine(solutionPath, "src", "Entity");

            // TODO:备份
            IOHelper.MoveDirectory(entitiesDir, destDir);

            // move .csproj
            var sourceProjectFile = Directory.GetFiles(Path.Combine(solutionPath, Config.EntityPath), "*.csproj", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();
            if (sourceProjectFile != null)
            {
                var destProjectFile = Path.Combine(destDir, "Entity.csproj");
                File.Move(sourceProjectFile, destProjectFile, true);
                await solution.AddExistProjectAsync(destProjectFile);
            }

            // create globaUsings
            var globalUsingPath = Path.Combine(destDir, "GlobalUsings.cs");
            File.WriteAllText(globalUsingPath, $"""
                global using System.ComponentModel;
                global using System.ComponentModel.DataAnnotations;
                global using Ater.Web.Core.Attributes;
                global using Ater.Web.Core.Models;
                global using Microsoft.EntityFrameworkCore;
                """, Encoding.UTF8);
            // delete old project
            Directory.Delete(Path.Combine(solutionPath, Config.EntityPath), true);

            // Share修改
            new List<string>() {
                "Models/PageList.cs",
                "Models/FilterBase.cs"
            }.ForEach(f =>
            {
                File.Delete(Path.Combine(solutionPath, Config.DtoPath, f));
            });
            var globalFilePath = Path.Combine(solutionPath, Config.DtoPath, "GlobalUsings.cs");
            File.AppendAllLines(globalFilePath, new List<string>() {
                "global using Ater.Web.Core.Models;"
            });

            // Application修改
            // 结构调整
            var applicationDir = Path.Combine(solutionPath, Config.ApplicationPath);
            var appAssemblyName = Config.ApplicationPath.Split('/').Last();
            await solution.MoveDocumentAsync(
                appAssemblyName,
                Path.Combine(applicationDir, "Interface", "IDomainManager.cs"),
                Path.Combine(applicationDir, "IManager", "IDomainManager.cs"),
                $"{appAssemblyName}.IManager");

            await solution.MoveDocumentAsync(
                appAssemblyName,
                Path.Combine(applicationDir, "Implement", "DataStoreContext.cs"),
                Path.Combine(applicationDir, "DataStoreContext.cs"),
                $"{appAssemblyName}");

            await solution.MoveDocumentAsync(
                appAssemblyName,
                Path.Combine(applicationDir, "Interface", "IUserContext.cs"),
                Path.Combine(applicationDir, "IUserContext.cs"),
                $"{appAssemblyName}");

            Directory.Delete(Path.Combine(applicationDir, "Interface"), true);

            // remove package reference
            string[] packageNames = new string[] {
                "Microsoft.IdentityModel.Tokens",
                "Microsoft.AspNetCore.Http.Abstractions",
                "System.IdentityModel.Tokens.Jwt",
                "Microsoft.EntityFrameworkCore.SqlServer",
                "Npgsql.EntityFrameworkCore.PostgreSQL",
                "Microsoft.Extensions.Caching.StackExchangeRedis",
                "OpenTelemetry",
                "OpenTelemetry.Exporter.Console",
                "OpenTelemetry.Exporter.OpenTelemetryProtocol",
                "OpenTelemetry.Exporter.OpenTelemetryProtocol.Logs",
                "OpenTelemetry.Extensions.Hosting",
                "OpenTelemetry.Instrumentation.AspNetCore",
                "OpenTelemetry.Instrumentation.Http",
                "OpenTelemetry.Instrumentation.SqlClient"
            };
            var appProjectFile = Directory.GetFiles(applicationDir, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (appProjectFile != null)
            {
                var commands = packageNames.Select(p => $"dotnet remove {appProjectFile} package " + p).ToArray();
                ProcessHelper.ExecuteCommands(commands);
            }

            // rename namespace
            solution.RenameNamespace("Core.Entities", "Entity");
            solution.RenameNamespace("Core.Models", "Ater.Web.Core.Models");
            solution.RenameNamespace("Core.Utils", "Ater.Web.Core.Utils");
            solution.RenameNamespace("Application.Interface", string.Empty);

            // 重构项目依赖关系

            var entityProject = solution.Projects.FirstOrDefault(p => p.Name == "Entity");
            var aterCoreProject = solution.Projects.FirstOrDefault(p => p.Name == aterCoreName);

            var applicationProject = solution.Projects.FirstOrDefault(p => p.Name == appAssemblyName);
            var aterAbstractureProject = solution.Projects.FirstOrDefault(p => p.Name == aterAbstracture);

            var dtoProject = solution.Projects.FirstOrDefault(p => p.Name == "Share");
            var entityFrameworkProject = solution.Projects.FirstOrDefault(p => p.Name == "EntityFramework");

            if (entityProject == null || aterCoreProject == null || applicationProject == null || aterAbstractureProject == null || dtoProject == null || entityFrameworkProject == null)
            {
                Console.WriteLine("⚠️ 项目依赖关系重构失败，缺失的项目关系:" +
                    "\nentityProject:" + entityProject?.Name +
                    "\naterCoreProject:" + aterCoreProject?.Name +
                    "\napplicationProject:" + applicationProject?.Name +
                    "\naterAbstractureProject:" + aterAbstractureProject?.Name +
                    "\ndtoProject:" + dtoProject?.Name +
                    "\nentityFrameworkProject:" + entityFrameworkProject?.Name
                    );
                return false;
            }

            solution.AddProjectRefrence(entityProject, aterCoreProject);
            solution.AddProjectRefrence(dtoProject, entityProject);
            solution.AddProjectRefrence(entityFrameworkProject, entityProject);
            solution.AddProjectRefrence(applicationProject, aterAbstractureProject);

            var saved = solution.Save();
            if (!saved)
            {
                await Console.Out.WriteLineAsync("Save solution failed!");
                return false;
            }

            // 配置文件等
            var configFile = Path.Combine(solutionPath, Config.ConfigFileName);
            var config = JsonSerializer.Deserialize<ConfigOptions>(File.ReadAllText(configFile));
            if (config != null)
            {
                config.EntityPath = "src/Entity";
                config.Version = "8.0.0";
                config.SolutionType = Core.Entities.SolutionType.DotNet;

                File.WriteAllText(configFile, JsonSerializer.Serialize(config, new JsonSerializerOptions()
                {
                    WriteIndented = true
                }));
                return true;
            }
            return false;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + ex.StackTrace);
            return false;
        }

    }
    #endregion
}
