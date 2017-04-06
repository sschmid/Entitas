using System;

namespace Entitas.CodeGeneration.Attributes {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class EntityIndexAttribute : AbstractEntityIndexAttribute {

        public EntityIndexAttribute() : base(EntityIndexType.EntityIndex) {
        }
    }
}
