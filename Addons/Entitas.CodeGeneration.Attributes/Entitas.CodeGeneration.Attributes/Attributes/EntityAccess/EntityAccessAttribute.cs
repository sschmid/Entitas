using System;

namespace Entitas.CodeGeneration.Attributes {

    public class AbstractEntityAccessAttribute : Attribute {

        public readonly EntityAccessType entityAccess;
        public readonly Type componentType;

        protected AbstractEntityAccessAttribute(EntityAccessType entityAccess,  Type componentType) {
            this.entityAccess = entityAccess;
            this.componentType = componentType;;
        }
    }
}
