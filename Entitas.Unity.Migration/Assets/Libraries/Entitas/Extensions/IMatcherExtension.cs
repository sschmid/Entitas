namespace Entitas {
    public static class IMatcherExtension {

        /// Convenience method to create a new TriggerOnEvent. Commonly used in IReactiveSystem and IMultiReactiveSystem.
        public static TriggerOnEvent OnEntityAdded(this IMatcher matcher) {
            return new TriggerOnEvent(matcher, GroupEventType.OnEntityAdded);
        }

        /// Convenience method to create a new TriggerOnEvent. Commonly used in IReactiveSystem and IMultiReactiveSystem.
        public static TriggerOnEvent OnEntityRemoved(this IMatcher matcher) {
            return new TriggerOnEvent(matcher, GroupEventType.OnEntityRemoved);
        }

        /// Convenience method to create a new TriggerOnEvent. Commonly used in IReactiveSystem and IMultiReactiveSystem.
        public static TriggerOnEvent OnEntityAddedOrRemoved(this IMatcher matcher) {
            return new TriggerOnEvent(matcher, GroupEventType.OnEntityAddedOrRemoved);
        }
    }
}

