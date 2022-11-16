using System;

namespace Entitas.CodeGeneration.Attributes
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
