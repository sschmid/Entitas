using System;

namespace Entitas.CodeGenerator.Api {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class EntityIndexAttribute : AbstractEntityIndexAttribute {

        public EntityIndexAttribute() : base(EntityIndexType.EntityIndex) {
        }
    }
}
