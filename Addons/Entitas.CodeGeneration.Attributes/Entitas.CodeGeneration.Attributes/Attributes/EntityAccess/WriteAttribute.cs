using System;

namespace Entitas.CodeGeneration.Attributes {

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct)]
    public class WriteAttribute : AbstractEntityAccessAttribute {

        public WriteAttribute(Type componentType) : base(EntityAccessType.Write, componentType) {
        }
    }
}
