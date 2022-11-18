using System;

namespace Entitas.Plugins.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = true)]
    public class ContextAttribute : Attribute
    {
        public readonly string Name;

        public ContextAttribute(string name)
        {
            Name = name;
        }
    }
}
