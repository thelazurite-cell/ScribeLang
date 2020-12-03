using System;
using System.Linq;
using Scribe.Interpreter.BluePrints;
using Scribe.Interpreter.BluePrints.Attributes;
using Scribe.Interpreter.BluePrints.Interfaces;

namespace Scribe.Interpreter.Core.Type.I
{
    [Keyword(Words = new[] {"i32", "int"})]
    public class I32 : ValueType<int>
    {
        public override bool SetValueByObject(object obj)
        {
            if (string.IsNullOrWhiteSpace(obj.ToString()))
                throw new RuntimeException("ERROR: Expected a Value");
            if (!int.TryParse(obj.ToString(), out _value))
                throw new RuntimeException("ERROR: Not a number");
            return true;
        }

        private int _value;

        public override int Value
        {
            get => _value;
            set => _value = value;
        }
    }
}