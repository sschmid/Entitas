namespace Entitas.CodeGeneration.Plugins {

    public class EventData {

        public readonly bool bindToEntity;
        public readonly int priority;

        public EventData(bool bindToEntity, int priority) {
            this.bindToEntity = bindToEntity;
            this.priority = priority;
        }
    }
}
