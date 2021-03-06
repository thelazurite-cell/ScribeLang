using System;
using Scribe.Interpreter.BluePrints;
using Scribe.Interpreter.BluePrints.Attributes;
using Scribe.Interpreter.BluePrints.Interfaces;

namespace Scribe.Interpreter.Core.Type.U
{
    [Keyword(Words = new []{"u16"})]
    public class U16:ValueType<ushort>
    {
        public override bool SetValueByObject(object obj)
        {
            if (string.IsNullOrWhiteSpace(obj.ToString()))
                throw new RuntimeException("ERROR: Expected a Value");
            if (!ushort.TryParse(obj.ToString(), out _value))
                throw new RuntimeException("ERROR: Not a number");
            return true;
        }

        private ushort _value;
        public override ushort Value { get => _value; set => _value=value; }
    }
}