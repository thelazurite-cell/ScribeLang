using System;
using Scribe.Interpreter.BluePrints.Attributes;
using Scribe.Interpreter.BluePrints.Interfaces;

namespace Scribe.Interpreter.Core.Flow
{
    [Keyword(Words = new []{"each",",",":","skip","finish"})]
    public class Each: IFlowControl
    {
        public string InstanceName { get; set; }
        public bool Validate(string[] lines)
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}