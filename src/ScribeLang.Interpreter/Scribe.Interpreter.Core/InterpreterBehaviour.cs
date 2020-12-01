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
using static Churva.Interpreter.BluePrints.ChurvaEnvironment;

namespace Churva.Interpreter.Core
{
    public class InterpreterBehaviour
    {
        private const string CoreName = "Scribe.Interpreter.Core.dll";
        private static Assembly _coreAsm;

        public static void Initialize()
        {
#if DEBUG
            Console.WriteLine("Loading keywords.");
#endif
            var executingDir = Assembly.GetExecutingAssembly().GetDirectory();
            _coreAsm = Assembly.LoadFile(
                Path.Combine(
                    executingDir,
                    CoreName
                )
            );
            LoadKeywords();
#if DEBUG
            Console.WriteLine("Loading Plugins");
#endif
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            };
            var files = Directory.GetFiles(Path.Combine(executingDir, "plugins")).Where(itm => itm.EndsWith(".json"));
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var plugin = JsonSerializer.Deserialize<Plugin>(json, options);
#if DEBUG
                Console.WriteLine(plugin.Alias);
#endif
                Plugins.Add(plugin);
            }
        }

        public static void ExecuteLine(string line)
        {
            line = line.Trim().Replace("\t", string.Empty);
            if (IsCreatingVariable(line)) return;
            var op = MultiOperators.FirstOrDefault(itm => itm.Value == MultiOperator.ClassAccessor).Key;
            if (IsCallingMethod(line, op))
            {
                AttemptToCallMethod(line, op);
            }
            else
            {
                throw new RuntimeException($"ERROR: Invalid Token - {line}");
            }
        }

        private static void AttemptToCallMethod(string line, string op)
        {
            var plg = Plugins.FirstOrDefault(itm => line.StartsWith($"{itm.Alias}{op}"));
            var tmp = line.Replace($"{plg.Alias}{op}", "");
            var mthd = plg.Methods.FirstOrDefault(itm => tmp.StartsWith($"{itm.Alias}("));
            tmp = tmp.Replace($"{mthd.Alias}", "");
            if (tmp.StartsWith("(") && tmp.EndsWith(")"))
            {
                var args = new List<string>();
                tmp = ProcessGroup(tmp, args);

                AttemptPluginExecution(plg, mthd, args.ToArray());
            }
            else
            {
                throw new RuntimeException("ERROR: Method params must be wrapped in (brackets).");
            }
        }

        private static bool IsCallingMethod(string line, string op)
        {
            return Plugins.Any(itm => line.StartsWith($"{itm.Alias}{op}"));
        }

        private static bool IsCreatingVariable(string line)
        {
            var matched = false;
            foreach (var key in Keywords.Keys)
            {
                var match = key?.Where(line.StartsWith).ToList();
                if (!(match ?? new List<string>()).Any()) continue;
                matched = true;
                var use = match.FirstOrDefault();
                var type = Keywords[key];
#if DEBUG
                Console.WriteLine($"Started with: {use}");
#endif
                var instance = Activator.CreateInstance(type);
                var val = instance.InvokeReturnMethod<bool>($"{nameof(IKeyword.Validate)}",
                    new object[] {new[] {line}});
#if DEBUG
                Console.WriteLine(val);
#endif
            }

            return matched;
        }

        private static string ProcessGroup(string tmp, List<string> args)
        {
            tmp = RemoveGroupingOperators(tmp);

            while (!string.IsNullOrWhiteSpace(tmp))
            {
                tmp = tmp.StartsWith("\"")
                    ? ProcessStringParameter(tmp, args)
                    : ProcessNonStringParameter(tmp, args);
            }

            return tmp;
        }

        private static string ProcessNonStringParameter(string tmp, List<string> args)
        {
            var separator = tmp.IndexOf(",", StringComparison.Ordinal);
            var value = tmp.Substring(0, separator == -1 ? tmp.Length : separator + 1);
            args.Add(value.Replace(",", string.Empty));
            tmp = tmp.Replace(value, string.Empty).Trim();
            return tmp;
        }

        public static string RemoveGroupingOperators(string tmp)
        {
            tmp = new string(
                tmp.Remove(
                    tmp.LastIndexOf(
                        MultiOperators.FirstOrDefault(itm => itm.Value == MultiOperator.GroupEnd).Key,
                        StringComparison.Ordinal), 1).Reverse().ToArray());
            tmp = new string(tmp
                .Remove(
                    tmp.LastIndexOf(
                        MultiOperators.FirstOrDefault(itm => itm.Value == MultiOperator.GroupStart).Key,
                        StringComparison.Ordinal), 1).Reverse().ToArray()).Trim();
            return tmp;
        }

        private static string ProcessStringParameter(string tmp, List<string> args)
        {
            var first = 0;
            while (first != -1)
            {
                first = tmp.IndexOf('"');
                if (first <= -1) continue;
                var escaped = true;
                var next = 0;
                var firstIter = true;
                while (escaped)
                {
                    var startIndex = (firstIter ? first : next) + 1;

                    if (startIndex > tmp.Length - 1)
                        throw new RuntimeException("ERROR: string not enclosed in quotation marks");

                    next = tmp.IndexOf('"', startIndex);
                    escaped = tmp[next - 1] == '\\';

                    if (next == -1)
                        throw new RuntimeException("ERROR: string not enclosed in quotation marks");

                    firstIter = false;
                }

                var sub = tmp.Substring(first, next + 1 > tmp.Length ? tmp.Length - 1 : next + 1);
                args.Add(sub.Replace("\\\"", "\"").Trim());
                tmp = tmp.Replace(sub, string.Empty).Trim();
                if (tmp.StartsWith(","))
                    tmp = tmp.Remove(0, 1).Trim();
            }

            return tmp;
        }

        private static void AttemptPluginExecution(Plugin plg, PluginMethod mthd, string[] args)
        {
            var asms = AppDomain.CurrentDomain.GetAssemblies();

            var hasRun = false;
            foreach (var asm in asms)
            {
                foreach (var t in asm.GetExportedTypes()
                    .Where(itm => itm?.Assembly?.FullName?.ToLower() == plg.AssemblyName.ToLower()))
                {
                    if (string.Equals(t.Name, plg.ClassName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var methods = t.GetMethods().Where(itm => itm.Name.ToLower() == mthd.MethodName.ToLower());
                        foreach (var method in methods)
                        {
                            var parameters = method.GetParameters();

                            var obj = new List<object>();
                            var matching = parameters.Length == args.Length;
                            if (!matching) continue;
                            for (var i = 0; i < parameters.Length; i++)
                            {
                                var parameter = parameters[i];
                                var access = SingleOperators
                                    .FirstOrDefault(itm => itm.Value == SingleOperator.VariableAccess).Key.ToString();

                                if (args[i].StartsWith(access))
                                {
                                    var tmp = args[i].Replace(access, "");
                                    if (GetInstance(tmp, out var instance))
                                    {
                                        if (instance.Value.GetType() != parameter.ParameterType)
                                        {
                                            matching = false;
                                            break;
                                        }

                                        obj.Add(instance.Value);
                                    }
                                    else
                                    {
                                        throw new RuntimeException(
                                            $"ERROR: No matching variable '{tmp}' in current scope");
                                    }
                                }
                                else if (i >= 0 && args.Length > i)
                                {
                                    if (parameter.ParameterType.ToString().ToLower() == "system.string")
                                    {
                                        if (args[i].StartsWith('"') && args[i].EndsWith('"'))
                                        {
                                            var tmp = args[i];
                                            tmp = RemoveContainingQuote(tmp);
                                            tmp = RemoveContainingQuote(tmp);
                                            obj.Add(tmp);
                                        }
                                        else
                                        {
                                            throw new RuntimeException("Error: string not wrapped in quotation marks");
                                        }
                                    }
                                    else
                                    {
                                        var typeAsm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(itm =>
                                            itm.GetTypes().Any(type => string.Equals(type.Name.ToString(),
                                                parameter.ParameterType.Name.ToString(),
                                                StringComparison.CurrentCultureIgnoreCase)));
                                        var type = typeAsm.GetTypes().FirstOrDefault(type =>
                                            string.Equals(type.Name.ToString(),
                                                parameter.ParameterType.Name.ToString(),
                                                StringComparison.CurrentCultureIgnoreCase));
                                        var tryParse = type.GetMethods().FirstOrDefault(itm => itm.Name == "TryParse");
                                        var parseParameters = new object[] {args[i], null};
                                        var parseResult = tryParse.Invoke(null, parseParameters);
                                        var blResult = parseResult != null && (bool) parseResult;
                                        if (blResult)
                                        {
                                            obj.Add(parseParameters[1]);
                                        }
                                        else
                                        {
                                            throw new RuntimeException("ERROR: param value non-convertible");
                                        }
                                    }
                                }
                            }

                            if (!matching) continue;
                            method.Invoke(t.IsAbstract ? null : Activator.CreateInstance(t), obj.ToArray());
                            hasRun = true;
                            break;
                        }
                    }

                    if (hasRun) break;
                }

                if (hasRun) break;
            }

            if (!hasRun)
                throw new RuntimeException(
                    $"ERROR: {plg.Alias} did not have a matching {mthd.Alias} with the provided parameter(s)");
        }

        private static string RemoveContainingQuote(string str)
        {
            return new string(str.Remove(str.LastIndexOf("\"", StringComparison.Ordinal), 1).Reverse().ToArray());
        }

        private static void LoadKeywords()
        {
            var coreTypes = _coreAsm.GetTypes();

            foreach (var type in coreTypes)
            {
                var attributeType = type.GetCustomAttribute<KeywordAttribute>();
                if (attributeType == null) continue;
                Keywords.Add(attributeType.Words, type);
#if DEBUG
                Console.WriteLine($"{type.Name} Loaded - {string.Join(",", attributeType.Words)}");
#endif
            }
        }
    }
}