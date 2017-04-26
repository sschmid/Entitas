using System;

namespace Entitas.CodeGeneration.Attributes {

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct)]
    public class ReadWriteAttribute : AbstractEntityAccessAttribute {

        public ReadWriteAttribute(Type componentType) : base(EntityAccessType.ReadWrite, componentType) {
        }
    }
}
