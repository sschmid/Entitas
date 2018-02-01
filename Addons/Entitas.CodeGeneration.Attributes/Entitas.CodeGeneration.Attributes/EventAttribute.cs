using System;

namespace Entitas.CodeGeneration.Attributes {

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public class EventAttribute : Attribute {

        public readonly bool bindToEntity;
        public readonly int priority;

        public EventAttribute(bool bindToEntity, int priority = 0) {
            this.bindToEntity = bindToEntity;
            this.priority = priority;
        }
    }
}
