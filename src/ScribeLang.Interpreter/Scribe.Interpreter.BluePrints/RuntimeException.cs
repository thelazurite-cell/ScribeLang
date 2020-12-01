using System;
using System.Runtime.InteropServices;

namespace Churva.Interpreter.BluePrints
{
    public class RuntimeException: Exception
    {
        public RuntimeException(string message) : base(message)
        {
        }
    }
}