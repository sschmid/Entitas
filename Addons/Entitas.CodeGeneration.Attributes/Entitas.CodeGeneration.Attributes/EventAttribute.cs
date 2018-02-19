using System;

namespace Entitas.CodeGeneration.Attributes {

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public class EventAttribute : Attribute {

        public readonly bool bindToEntity;
        public readonly EventType eventType;
        public readonly int priority;

        public EventAttribute(bool bindToEntity, EventType eventType = EventType.Added, int priority = 0) {
            this.bindToEntity = bindToEntity;
            this.eventType = eventType;
            this.priority = priority;
        }
    }

    public enum EventType {
        Added,
        Removed,
        AddedOrRemoved
    }
}
