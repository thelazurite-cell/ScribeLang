using System;

namespace Churva.Interpreter.BluePrints.Interfaces
{
    public interface IKeyword
    {
        string InstanceName { get; set; }
        bool Validate(string[] strings);
    }
}