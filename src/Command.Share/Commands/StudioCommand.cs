﻿using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Command.Share.Commands;
public class StudioCommand
{
    public static void RunStudio()
    {
        Console.WriteLine("welcome ater studio!");
        string appPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        string version = Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;

        string targetFramework = "net7.0";
        string packageId = "ater.droplet.cli";

        string zipPath = Path.Combine(userPath, ".dotnet/tools/.store", packageId, version, packageId, version, "tools", targetFramework, "any", "studio.zip");


        var studioPath = Path.Combine(appPath, "AterStudio");
        var file = new FileInfo(Path.Combine(studioPath, Config.StudioFileName));

        if (!file.Exists)
        {
            // 初始化，解压内容
            Console.WriteLine($"init studio...");
            if (!File.Exists(zipPath))
            {
                Console.WriteLine("not found studio.zip");
                return;
            }
            ZipFile.ExtractToDirectory(zipPath, studioPath, true);
            file = new(Path.Combine(studioPath, Config.StudioFileName));
        }

        Console.WriteLine("start studio...");
        // 运行
        string path = file.FullName;
        string shell = "dotnet";
        //switch (Environment.OSVersion.Platform)
        //{
        //    case PlatformID.Unix:
        //        shell = "bash";
        //        break;
        //    default:
        //        break;
        //}

        var url = "http://localhost:9160";
        Process process = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = shell,
                Arguments = $"./{Config.StudioFileName} --urls \"{url}\"",
                UseShellExecute = true,
                CreateNoWindow = true,
                //RedirectStandardOutput = true,
                WorkingDirectory = studioPath
                //RedirectStandardError = true,
                //StandardErrorEncoding = Encoding.UTF8,
                //StandardOutputEncoding = Encoding.UTF8,
            }
        };

        var startStudio = process.Start();
        Thread.Sleep(3000);
        // 启动浏览器
        if (startStudio)
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception ex)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}")
                    {
                        CreateNoWindow = true
                    });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    Console.WriteLine("start browserr failed:" + ex.Message);
                }
            }
        }

        //Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) =>
        //{
        //    var isCtrlC = e.SpecialKey == ConsoleSpecialKey.ControlC;
        //    var isCtrlBreak = e.SpecialKey == ConsoleSpecialKey.ControlBreak;

        //    if (isCtrlBreak || isCtrlC)
        //    {
        //        process.Close();
        //        process.Dispose();
        //    }
        //};
        process.WaitForExit();
    }


    /// <summary>
    /// 升级studio
    /// </summary>
    public static void Update()
    {
        Console.WriteLine($"updating studio...");

        var copyFiles = new string[] { "Microsoft.CodeAnalysis.CSharp", "Microsoft.CodeAnalysis", "LiteDB", "SharpYaml", "Microsoft.OpenApi", "CodeGenerator", "Microsoft.OpenApi.Readers", "Core", "Command.Share", "Datastore" };

        string appPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        string version = Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;

        string targetFramework = "net7.0";
        string packageId = "ater.droplet.cli";

        var toolRootPath = Path.Combine(userPath, ".dotnet/tools/.store", packageId, version, packageId, version, "tools", targetFramework, "any");
        string zipPath = Path.Combine(toolRootPath, "studio.zip");

        if (!File.Exists(zipPath))
        {
            Console.WriteLine("not found studio.zip");
            return;
        }
        // 解压
        ZipFile.ExtractToDirectory(zipPath, Path.Combine(appPath, "AterStudio"), true);
        // copy其他文件以及runtimes目录
        copyFiles.ToList().ForEach(file =>
        {
            var sourceFile = Path.Combine(toolRootPath, file + ".dll");
            if (File.Exists(sourceFile))
            {
                File.Copy(sourceFile, Path.Combine(appPath, "AterStudio", file + ".dll"), true);
            }
        });

        //CopyDirectory(Path.Combine(toolRootPath, "runtimes"), Path.Combine(appPath, "AterStudio", "runtimes"), true);

        Console.WriteLine("update complete!");
    }

    public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        var dir = new DirectoryInfo(sourceDir);

        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath, true);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }
}
