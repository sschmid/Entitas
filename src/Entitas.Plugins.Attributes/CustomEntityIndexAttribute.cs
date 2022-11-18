using System;

namespace Entitas.Plugins.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class CustomEntityIndexAttribute : Attribute
    {
        public readonly Type Type;

        public CustomEntityIndexAttribute(Type type)
        {
            Type = type;
        }
    }
}
