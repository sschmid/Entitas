using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public class EventData {

        public readonly bool bindToEntity;
        public readonly EventType eventType;
        public readonly int priority;

        public EventData(bool bindToEntity, EventType eventType, int priority) {
            this.bindToEntity = bindToEntity;
            this.eventType = eventType;
            this.priority = priority;
        }
    }
}
