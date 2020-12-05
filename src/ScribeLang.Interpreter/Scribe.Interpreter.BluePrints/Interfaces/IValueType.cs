using System;

namespace Scribe.Interpreter.BluePrints.Interfaces
{
    public interface IValueType: IKeyword
    {
        object Value { get; set; }
        bool SetValueByObject(object obj);
#pragma warning disable 108,114
        // Required not to be newed up for reflection targeting
        bool Validate(string[] lines);
#pragma warning restore 108,114
    }
    
    public interface IValueType<T>: IValueType
    {
        T Value { get; set; }
    }
}