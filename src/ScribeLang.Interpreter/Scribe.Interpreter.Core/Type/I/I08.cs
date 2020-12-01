using System;
using System.Linq;
using Churva.Interpreter.BluePrints;
using Churva.Interpreter.BluePrints.Attributes;
using Churva.Interpreter.BluePrints.Interfaces;

namespace Churva.Interpreter.Core.Type.I
{
    [Keyword(Words = new[] {"i08"})]
    public class I08 : ValueType<short>
    {
        public override bool SetValueByObject(object obj)
        {
            if (string.IsNullOrWhiteSpace(obj.ToString()))
                throw new RuntimeException("ERROR: Expected a Value");
            if (!short.TryParse(obj.ToString(), out var value))
            {
                throw new RuntimeException("ERROR: Not a number");
            }
            Value = value;

            return true;
        }

        private short _value;

        /// <summary>
        /// Gets or sets the value of an 8 bit number;
        /// </summary>
        /// <exception cref="RuntimeException"></exception>
        public override short Value
        {
            get => _value;
            set
            {
                if (value < -128 || value > 127)
                    throw new RuntimeException($"ERROR: Value is out of the max range for {nameof(I08)}");
                _value = value;
            }
        }
    }
}