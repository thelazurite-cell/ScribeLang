using System;
using Churva.Interpreter.BluePrints.Attributes;
using Churva.Interpreter.BluePrints.Interfaces;

namespace Churva.Interpreter.Core.Flow
{
    [Keyword(Words = new[]{"sub"})]
    public class Sub : IFlowControl
    {
        public string InstanceName { get; set; }

        public bool Validate(string[] strings)
        {
            throw new System.NotImplementedException();
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}