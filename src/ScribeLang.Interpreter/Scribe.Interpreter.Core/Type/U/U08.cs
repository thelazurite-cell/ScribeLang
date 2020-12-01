using System;
using Churva.Interpreter.BluePrints;
using Churva.Interpreter.BluePrints.Attributes;
using Churva.Interpreter.BluePrints.Interfaces;

namespace Churva.Interpreter.Core.Type.U
{
    [Keyword(Words = new []{"u08"})]
    public class U08 : ValueType<ushort>
    {
        private ushort _value;

        public override bool SetValueByObject(object obj)
        {
            if (string.IsNullOrWhiteSpace(obj.ToString()))
                throw new RuntimeException("ERROR: Expected a Value");
            if (!ushort.TryParse(obj.ToString(), out var value))
                throw new RuntimeException("ERROR: Not a number");
            Value = value;
            return true;
        }

        public override ushort Value
        {
            get => _value;
            set
            {
                if (value > 255)
                    throw new RuntimeException($"Value is out of the max range for {nameof(U08)}");
                _value = value;
            }
        }
    }
}