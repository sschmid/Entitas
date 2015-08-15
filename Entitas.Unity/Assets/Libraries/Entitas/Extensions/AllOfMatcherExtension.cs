namespace Entitas {
    public static class AllOfMatcherExtension {
        public static TriggerOnEvent OnEntityAdded(this AllOfMatcher matcher) {
            return new TriggerOnEvent(matcher, GroupEventType.OnEntityAdded);
        }

        public static TriggerOnEvent OnEntityRemoved(this AllOfMatcher matcher) {
            return new TriggerOnEvent(matcher, GroupEventType.OnEntityRemoved);
        }

        public static TriggerOnEvent OnEntityAddedOrRemoved(this AllOfMatcher matcher) {
            return new TriggerOnEvent(matcher, GroupEventType.OnEntityAddedOrRemoved);
        }
    }
}

