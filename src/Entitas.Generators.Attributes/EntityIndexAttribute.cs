using System;

namespace Entitas.Generators.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EntityIndexAttribute : Attribute
    {
        public readonly bool IsPrimary;

        public EntityIndexAttribute(bool isPrimary)
        {
            IsPrimary = isPrimary;
        }
    }
}
