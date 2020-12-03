using System;
using System.Linq;
using Scribe.Interpreter.BluePrints;
using Scribe.Interpreter.BluePrints.Attributes;
using Scribe.Interpreter.BluePrints.Interfaces;

namespace Scribe.Interpreter.Core.Type.I
{
    [Keyword(Words = new[] {"i64"})]
    public class I64 : ValueType<long>
    {
        public override bool SetValueByObject(object obj)
        {
            if (string.IsNullOrWhiteSpace(obj.ToString()))
                throw new RuntimeException("ERROR: Expected a Value");
            if (!long.TryParse(obj.ToString(), out _value))
                throw new RuntimeException("ERROR: Not a number");
            return true;
        }

        private long _value;

        public override long Value
        {
            get => _value;
            set => _value = +value;
        }
    }
}