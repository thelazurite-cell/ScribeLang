using System;
using Churva.Interpreter.BluePrints;
using Churva.Interpreter.BluePrints.Attributes;
using Churva.Interpreter.BluePrints.Interfaces;

namespace Churva.Interpreter.Core.Type
{
    [Keyword(Words = new []{"str"})]
    public class Str: ValueType<string>
    {
        public override bool SetValueByObject(object obj)
        {
            var val = obj.ToString();
            if(!(val.StartsWith("\"") && val.EndsWith("\"")))
                throw new RuntimeException($"ERROR: {InstanceName} Expected \" => {val}");

            val = val.Substring(1, val.Length - 1);
            val = val.Remove(val.Length - 1, 1);
            Value = val;
            return true;
        }

        public override string Value { get; set; }
    }
}