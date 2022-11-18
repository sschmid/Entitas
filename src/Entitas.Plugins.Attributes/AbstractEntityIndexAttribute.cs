using System;

namespace Entitas.Plugins.Attributes
{
    public abstract class AbstractEntityIndexAttribute : Attribute
    {
        public readonly EntityIndexType Type;

        protected AbstractEntityIndexAttribute(EntityIndexType type)
        {
            Type = type;
        }
    }
}
