namespace Entitas {

    public static class ContextExtension {

        /// Returns all entities matching the specified matcher.
        public static TEntity[] GetEntities<TEntity>(this IContext<TEntity> context, IMatcher<TEntity> matcher) where TEntity : class, IEntity {
            return context.GetGroup(matcher).GetEntities();
        }

        /// Creates a new entity and adds copies of all
        /// specified components to it.
        /// If replaceExisting is true it will replace exisintg components.
        public static TEntity CloneEntity<TEntity>(this IContext<TEntity> context,
            IEntity entity,
            bool replaceExisting = false,
            params int[] indices)
            where TEntity : class, IEntity {
            var target = context.CreateEntity();
            entity.CopyTo(target, replaceExisting, indices);
            return target;
        }
    }
}
