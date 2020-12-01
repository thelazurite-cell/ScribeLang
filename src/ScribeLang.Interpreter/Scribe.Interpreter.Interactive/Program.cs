using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Churva.Interpreter.BluePrints;
using Churva.Interpreter.BluePrints.Attributes;
using Churva.Interpreter.BluePrints.Interfaces;
using Churva.Interpreter.BluePrints.Plugins;
using Churva.Interpreter.Core;

namespace Churva.Interpreter.Interactive
{
    /// <summary>
    /// The main class of the application
    /// </summary>
    internal class Program
    {

        private static bool multiLine = false;
        private static string _quit = ":q";
        private static string _run = ":r";

        /// <summary>
        /// The main entry point of the application
        /// </summary>
        /// <param name="args">The parameters provided by the user</param>
        private static void Main(string[] args)
        {
            InterpreterBehaviour.Initialize();
            LaunchCli();
        }

        private static void JsonCreate()
        {
            foreach (Assembly target in AppDomain.CurrentDomain.GetAssemblies())
            {
                DumpClassToJson(target);
            }
        }

        private static void LaunchCli()
        {
            var currentLine = string.Empty;
            var lines = new List<string>();
            while (currentLine != _quit)
            {
                try
                {
                    Console.Write(": ");
                    currentLine = Console.ReadLine() ?? "";
                    if (currentLine == _quit)
                        break;
                    if ((multiLine && currentLine != _run) || !multiLine)
                    {
                        lines.Add(currentLine);
                    }

                    if ((!multiLine || currentLine != _run) && multiLine) continue;
                    foreach (var line in lines)
                    {
                        InterpreterBehaviour.ExecuteLine(line);
                    }

                    lines.Clear();
                }
                catch (TargetInvocationException e)
                {
                    lines.Clear();
                    if (e.InnerException != null) Console.WriteLine($"{e.InnerException.Message}");
                }
                catch (RuntimeException e)
                {
                    lines.Clear();
                    Console.WriteLine($"{e.Message}");
                }
            }
        }

        private static void DumpClassToJson(Assembly target)
        {
            foreach (Type t in target.GetExportedTypes())
            {
                if (t.Name == "Console")
                {
                    Console.WriteLine("{");
                    Console.WriteLine($"\"ClassName\": \"{t.Name}\",");
                    Console.WriteLine($"\"Alias\": \"{t.Name}\",");
                    Console.WriteLine($"\"AssemblyName\": \"{t.Assembly.FullName}\",");
                    Console.WriteLine($"\"Methods\": [");
                    foreach (var method in t.GetMethods().Where(itm => itm.IsStatic))
                    {
                        Console.WriteLine("{");
                        Console.WriteLine($"\"MethodName\": \"{method.Name}\",");
                        Console.WriteLine($"\"Alias\": \"{method.Name}\",");
                        Console.WriteLine($"\"Parameters\": [");
                        foreach (var parameter in method.GetParameters())
                        {
                            Console.WriteLine("{");
                            Console.WriteLine($"\"Name\": \"{parameter.Name}\",");
                            Console.WriteLine($"\"Type\": \"{parameter.ParameterType.Name}\"");
                            Console.WriteLine("},");
                        }

                        Console.WriteLine("]");
                        Console.WriteLine("},");
                    }

                    Console.WriteLine("]");
                    Console.WriteLine("}");
                    Console.ReadLine();
                    Console.Clear();
                    break;
                }
            }
        }
    }
}