using System;
using System.Runtime.InteropServices;

namespace Scribe.Interpreter.BluePrints
{
    public class RuntimeException: Exception
    {
        public RuntimeException(string message) : base(message)
        {
        }
    }
}