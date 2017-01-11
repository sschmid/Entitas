namespace Entitas {

    public static class ContextExtension {

        /// Returns all entities matching the specified matcher.
        public static IEntity[] GetEntities(this Context context, IMatcher matcher) {
            return context.GetGroup(matcher).GetEntities();
        }

        /// Creates an Collector.
        public static Collector CreateCollector(this Context context, IMatcher matcher, GroupEvent groupEvent = GroupEvent.Added) {
            return new Collector(context.GetGroup(matcher), groupEvent);
        }

        /// Creates a new entity and adds copies of all
        /// specified components to it.
        /// If replaceExisting is true it will replace exisintg components.
        public static IEntity CloneEntity(this Context context,
                                          IEntity entity,
                                          bool replaceExisting = false,
                                          params int[] indices) {
            var target = context.CreateEntity();
            entity.CopyTo(target, replaceExisting, indices);
            return target;
        }
    }
}
