using System;
using System.Linq;
using System.Runtime.CompilerServices;
using static Scribe.Interpreter.BluePrints.ScribeEnvironment;

namespace Scribe.Interpreter.BluePrints.Interfaces
{
    public abstract class ValueType<T> : IValueType<T>
    {
        public ValueType()
        {
        }

        public ValueType(string instanceName, T value)
        {
            this.InstanceName = instanceName;
            this.Value = value;
        }

        object IValueType.Value
        {
            get => this.Value;
            set
            {
                if (value is T val) this.Value = val;
            }
        }

        public abstract bool SetValueByObject(object obj);

        public virtual T Value { get; set; }
        public virtual string InstanceName { get; set; }

        public virtual bool Validate(string[] lines)
        {
            if (!lines.Any()) return false;
            var ret = ParseValueInstance(this, lines);
            this.InstanceName = ret[0];
            if (this.SetValueByObject(ret[1]))
            {
#if DEBUG
                Console.WriteLine($"{this.InstanceName}={this.Value.ToString()}");
#endif
            }
            Variables.Add(this, new ItemScope(CurrentScope));
            return true;
        }
    }
}