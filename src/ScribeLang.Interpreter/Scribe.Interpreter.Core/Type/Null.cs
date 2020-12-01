using System;
using Churva.Interpreter.BluePrints.Attributes;
using Churva.Interpreter.BluePrints.Interfaces;

namespace Churva.Interpreter.Core.Type
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