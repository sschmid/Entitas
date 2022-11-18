using System;

namespace Entitas.Plugins.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class EntityIndexAttribute : AbstractEntityIndexAttribute
    {
        public EntityIndexAttribute() : base(EntityIndexType.EntityIndex) { }
    }
}
