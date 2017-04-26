using System;

namespace Entitas.CodeGeneration.Attributes {

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct)]
    public class ReadAttribute : AbstractEntityAccessAttribute {

        public ReadAttribute(Type componentType) : base(EntityAccessType.Read, componentType) {
        }
    }
}
