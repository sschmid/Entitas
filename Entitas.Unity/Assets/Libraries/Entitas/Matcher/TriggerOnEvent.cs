namespace Entitas {
    public struct TriggerOnEvent {
        public IMatcher trigger;
        public GroupEventType eventType;

        public TriggerOnEvent(IMatcher trigger, GroupEventType eventType) {
            this.trigger = trigger;
            this.eventType = eventType;
        }
    }
}

