namespace Entitas {
    public static class AllOfMatcherExtension {
        public static TriggerOnEvent OnEntityAdded(this IMatcher matcher) {
            return new TriggerOnEvent(matcher, GroupEventType.OnEntityAdded);
        }

        public static TriggerOnEvent OnEntityRemoved(this IMatcher matcher) {
            return new TriggerOnEvent(matcher, GroupEventType.OnEntityRemoved);
        }

        public static TriggerOnEvent OnEntityAddedOrRemoved(this IMatcher matcher) {
            return new TriggerOnEvent(matcher, GroupEventType.OnEntityAddedOrRemoved);
        }
    }
}

