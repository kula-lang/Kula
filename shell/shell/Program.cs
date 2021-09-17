﻿using System;
using System.Collections.Generic;
using System.IO;

using Kula;

class Program
{
    private static readonly KulaEngine kulaEngine = new KulaEngine();
    private static bool mode = false;
    
    private delegate void ShellCommand();
    private static readonly Dictionary<string, ShellCommand> ShellCommandDict = new Dictionary<string, ShellCommand>()
    {
        {"", () => { } },
        {"#debug", () => { mode = true; Console.WriteLine("Kula - Debug - Mode"); } },
        {"#release", () => { mode = false; Console.WriteLine("Kula - Release - Mode");} },
        {"#gomo", () => { Hello(); } },
        {"#clear", () => { kulaEngine.Clear(); } },
    };

    private delegate void ShellArgument();
    private static readonly Dictionary<string, ShellArgument> ShellArgumentDict = new Dictionary<string, ShellArgument>()
    {
        {"--debug", () => { mode = true; } },
        {"--d", () => { mode = true; } },

        {"--release", () => { mode = false; } },
        {"--r", () => { mode = false; } },
    };

    private static void Repl()
    {
        string code;
        while (true)
        {
            Console.Write(">> ");
            code = Console.ReadLine();
            if (code == "#exit")
            {
                break;
            }
            else if (ShellCommandDict.ContainsKey(code))
            {
                ShellCommandDict[code]();
            }
            else
            {
                try
                {
                    kulaEngine.Compile(code, "", mode);
                    kulaEngine.Run("", mode);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.GetType().ToString() + " : " + e/*.Message*/);
                    Console.ResetColor();
                }
            }
        }
    }

    private static void Hello()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(KulaEngine.Version + " (on .net Framework at least 4.6)");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("developed by @HanaYabuki on github.com");

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("More Info - ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("https://github.com/kula-lang/Kula");

        Console.ResetColor();
    }

    private static void CompileAndRun(string code)
    {
        kulaEngine.Compile(code, "", mode);
        kulaEngine.Run("", mode);
    }

    private static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            string code;
            foreach (string arg in args)
            {
                if (arg.StartsWith("--") && ShellArgumentDict.ContainsKey(arg))
                {
                    ShellArgumentDict[arg]();
                }
                else
                {
                    try
                    {
                        code = File.ReadAllText(arg);
                        CompileAndRun(code);
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(e);
                        Console.ResetColor();
                        return;
                    }
                }
            }

        }
        else
        {
            Hello();
            Repl();
        }
    }
}