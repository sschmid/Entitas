using System;

namespace Entitas.CodeGenerator.Api {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EntityIndexAttribute : AbstractEntityIndexAttribute {

        public EntityIndexAttribute() : base("EntityIndex", false) {
        }
    }
}
