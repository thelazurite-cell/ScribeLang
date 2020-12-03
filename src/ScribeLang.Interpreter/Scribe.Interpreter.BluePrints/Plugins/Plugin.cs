using System.Collections.Generic;

namespace Scribe.Interpreter.BluePrints.Plugins
{
    public class Plugin
    {
        public string ClassName { get; set; }
        public string Alias { get; set; }
        public string AssemblyName { get; set; }
        public List<PluginMethod> Methods { get; set; }
    }

    public class PluginMethod
    {
        public string MethodName { get; set; }
        public string Alias { get; set; }
        public List<PluginMethodParam> Parameters { get; set; }
    }

    public class PluginMethodParam
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}