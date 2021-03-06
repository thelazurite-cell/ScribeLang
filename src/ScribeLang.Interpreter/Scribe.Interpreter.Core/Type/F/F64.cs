using System;
using Scribe.Interpreter.BluePrints;
using Scribe.Interpreter.BluePrints.Attributes;
using Scribe.Interpreter.BluePrints.Interfaces;

namespace Scribe.Interpreter.Core.Type.F
{
    [Keyword(Words = new[] {"f64"})]
    public class F64 : ValueType<double>
    {
        public override bool SetValueByObject(object obj)
        {
            if (string.IsNullOrWhiteSpace(obj.ToString()))
                throw new RuntimeException("ERROR: Expected a Value");
            if (!double.TryParse(obj.ToString(), out _value))
                throw new RuntimeException("ERROR: Not a number");
            return true;
        }

        private double _value;

        public override double Value
        {
            get => _value;
            set => _value = value;
        }
    }
}