using System;

namespace Entitas.Generators.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ContextAttribute : Attribute
    {
        public readonly Type Type;

        public ContextAttribute(Type type)
        {
            Type = type;
        }
    }
}
