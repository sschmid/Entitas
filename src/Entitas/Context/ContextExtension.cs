namespace Entitas
{
    public static class ContextExtension
    {
        /// Returns all entities matching the specified matcher.
        public static TEntity[] GetEntities<TEntity>(this IContext<TEntity> context, IMatcher<TEntity> matcher) where TEntity : Entity =>
            context.GetGroup(matcher).GetEntities();

        /// Creates a new entity and adds copies of all
        /// specified components to it.
        /// If replaceExisting is true it will replace exising components.
        public static TEntity CloneEntity<TEntity>(this IContext<TEntity> context,
            Entity entity,
            bool replaceExisting = false,
            params int[] indexes)
            where TEntity : Entity
        {
            var target = context.CreateEntity();
            entity.CopyTo(target, replaceExisting, indexes);
            return target;
        }
    }
}
