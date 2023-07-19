using System;

namespace Entitas.Generators.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ContextInitializationAttribute : Attribute
    {
        public readonly Type Type;

        public ContextInitializationAttribute(Type type)
        {
            Type = type;
        }
    }
}
