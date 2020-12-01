using System;
using Churva.Interpreter.BluePrints;
using Churva.Interpreter.BluePrints.Attributes;
using Churva.Interpreter.BluePrints.Interfaces;

namespace Churva.Interpreter.Core.Type.F
{
    [Keyword(Words = new[] {"dec"})]
    public class F128 : ValueType<decimal>
    {
        private decimal _value;

        public override bool SetValueByObject(object obj)
        {
            if (string.IsNullOrWhiteSpace(obj.ToString()))
                throw new RuntimeException("ERROR: Expected a Value");
            if (!decimal.TryParse(obj.ToString(), out _value))
                throw new RuntimeException("ERROR: Not a number");
            return true;
        }

        public override decimal Value
        {
            get => _value;
            set => _value = value;
        }
    }
}