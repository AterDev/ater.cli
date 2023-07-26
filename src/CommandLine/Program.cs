﻿using System.CommandLine;
using System.Text;

namespace Droplet.CommandLine;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        if (args.Length == 0)
        {
            return 0;
        }

        ShowLogo();
        await ConfigCommand.InitConfigFileAsync();
        RootCommand root = new CommandBuilder().Build();
        return await root.InvokeAsync(args);
    }

    private static void ShowLogo()
    {
        var logo = """
             _____    _____   __     __
            |  __ \  |  __ \  \ \   / /
            | |  | | | |__) |  \ \_/ / 
            | |  | | |  _  /    \   /  
            | |__| | | | \ \     | |   
            |_____/  |_|  \_\    |_|

                     -- for freedom --
            """;

        Console.WriteLine(logo);
    }
}
