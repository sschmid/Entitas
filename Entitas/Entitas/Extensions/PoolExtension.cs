namespace Entitas {

    public static class PoolExtension {

        /// Returns all entities matching the specified matcher.
        public static Entity[] GetEntities(this Pool pool, IMatcher matcher) {
            return pool.GetGroup(matcher).GetEntities();
        }

        public static EntityCollector CreateEntityCollector(this Pool pool, IMatcher matcher, GroupEventType eventType = GroupEventType.OnEntityAdded) {
            return new EntityCollector(pool.GetGroup(matcher), eventType);
        }

        /// Creates an EntityCollector which observes all specified pools.
        /// This is useful when you want to create an EntityCollector
        /// for multiple pools which can be used with IReactiveSystem.
        public static EntityCollector CreateEntityCollector(
            this Pool[] pools,
            IMatcher matcher,
            GroupEventType eventType = GroupEventType.OnEntityAdded) {
            var groups = new Group[pools.Length];
            var eventTypes = new GroupEventType[pools.Length];

            for (int i = 0; i < pools.Length; i++) {
                groups[i] = pools[i].GetGroup(matcher);
                eventTypes[i] = eventType;
            }

            return new EntityCollector(groups, eventTypes);
        }

        /// Creates a new entity and adds copies of all
        /// specified components to it.
        /// If replaceExisting is true it will replace exisintg components.
        public static Entity CloneEntity(this Pool pool,
                                         Entity entity,
                                         bool replaceExisting = false,
                                         params int[] indices) {
            var target = pool.CreateEntity();
            entity.CopyTo(target, replaceExisting, indices);
            return target;
        }
    }
}
