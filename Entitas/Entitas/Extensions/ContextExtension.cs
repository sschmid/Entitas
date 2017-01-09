namespace Entitas {

    public static class ContextExtension {

        /// Returns all entities matching the specified matcher.
        public static Entity[] GetEntities(this Context context, IMatcher matcher) {
            return context.GetGroup(matcher).GetEntities();
        }

        /// Creates an EntityCollector.
        public static EntityCollector CreateCollector(this Context context, IMatcher matcher, GroupEventType eventType = GroupEventType.OnEntityAdded) {
            return new EntityCollector(context.GetGroup(matcher), eventType);
        }

        /// Creates a new entity and adds copies of all
        /// specified components to it.
        /// If replaceExisting is true it will replace exisintg components.
        public static Entity CloneEntity(this Context context,
                                         Entity entity,
                                         bool replaceExisting = false,
                                         params int[] indices) {
            var target = context.CreateEntity();
            entity.CopyTo(target, replaceExisting, indices);
            return target;
        }
    }
}
