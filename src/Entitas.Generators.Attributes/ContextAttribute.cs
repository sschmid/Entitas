using System;

namespace Entitas.Generators.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ContextAttribute : Attribute
    {
        public readonly string Type;

        public ContextAttribute(string type)
        {
            Type = type;
        }
    }
}
