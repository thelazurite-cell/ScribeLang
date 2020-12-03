using Scribe.Interpreter.BluePrints;
using Scribe.Interpreter.BluePrints.Attributes;
using Scribe.Interpreter.BluePrints.Interfaces;

namespace Scribe.Interpreter.Core.Type
{
    [Keyword(Words = new[] {"boo","bool","true","false"})]
    public class Boo : ValueType<bool>
    {
        private bool _value;

        public override bool SetValueByObject(object obj)
        {
            if (string.IsNullOrWhiteSpace(obj.ToString()))
                throw new RuntimeException("ERROR: Expected a Value");
            if (!bool.TryParse(obj.ToString(), out this._value))
                throw new RuntimeException("ERROR: Not a number");
            return true;
        }

        public override bool Value
        {
            get => this._value;
            set => this._value = value;
        }
    }
}