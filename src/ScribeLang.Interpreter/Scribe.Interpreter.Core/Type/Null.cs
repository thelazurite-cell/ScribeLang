using System;
using Scribe.Interpreter.BluePrints.Attributes;
using Scribe.Interpreter.BluePrints.Interfaces;

namespace Scribe.Interpreter.Core.Type
{
    [Keyword(Words = new []{"nul"})]
    public class Nul: ValueType<object>, IKeyword
    {
        public string InstanceName { get; set; }

        public override bool SetValueByObject(object obj)
        {
            throw new NotImplementedException();
        }

        public override object Value { get; set; } = null;
    }
}