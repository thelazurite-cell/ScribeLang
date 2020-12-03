using System;
using Scribe.Interpreter.BluePrints;
using Scribe.Interpreter.BluePrints.Attributes;
using Scribe.Interpreter.BluePrints.Interfaces;

namespace Scribe.Interpreter.Core.Type.U
{
    [Keyword(Words = new[] {"u64"})]
    public class U64 : ValueType<ulong>
    {
        public override bool SetValueByObject(object obj)
        {
            if (string.IsNullOrWhiteSpace(obj.ToString()))
                throw new RuntimeException("ERROR: Expected a Value");
            if (!ulong.TryParse(obj.ToString(), out _value))
                throw new RuntimeException("ERROR: Not a number");
            return true;
        }

        private ulong _value;

        public override ulong Value
        {
            get => _value;
            set => _value = value;
        }
    }
}