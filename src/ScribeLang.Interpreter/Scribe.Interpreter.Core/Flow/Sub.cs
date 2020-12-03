using System;
using Scribe.Interpreter.BluePrints.Attributes;
using Scribe.Interpreter.BluePrints.Interfaces;

namespace Scribe.Interpreter.Core.Flow
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