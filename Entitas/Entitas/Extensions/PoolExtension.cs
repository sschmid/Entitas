namespace Entitas {

    public static class PoolExtension {

        /// Returns all entities matching the specified matcher.
        public static Entity[] GetEntities(this Pool pool, IMatcher matcher) {
            return pool.GetGroup(matcher).GetEntities();
        }

        /// Creates an EntityCollector.
        public static EntityCollector CreateCollector(this Pool pool, IMatcher matcher, GroupEventType eventType = GroupEventType.OnEntityAdded) {
            return new EntityCollector(pool.GetGroup(matcher), eventType);
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
