namespace Entitas {
    public static class PoolExtension {
        public static Entity[] GetEntities(this Pool pool, IMatcher matcher) {
            return pool.GetGroup(matcher).GetEntities();
        }

        public static PoolObserver CreateObserver(this Pool pool, IMatcher matcher, GroupEventType eventType = GroupEventType.OnEntityAdded) {
            return new PoolObserver(pool, matcher, eventType);
        }

        public static WillBeRemovedPoolObserver CreateWillBeRemovedObserver(this Pool pool, AllOfMatcher matcher) {
            return new WillBeRemovedPoolObserver(pool, matcher);
        }
    }
}

