namespace Entitas {
    public static class ContextExtension {
        public static Entity[] GetEntities(this Context context, IMatcher matcher) {
            return context.GetGroup(matcher).GetEntities();
        }

        public static ContextObserver CreateObserver(this Context context, IMatcher matcher, GroupEventType eventType = GroupEventType.OnEntityAdded) {
            return new ContextObserver(context, matcher, eventType);
        }

        public static WillBeRemovedContextObserver CreateWillBeRemovedObserver(this Context context, AllOfMatcher matcher) {
            return new WillBeRemovedContextObserver(context, matcher);
        }
    }
}

