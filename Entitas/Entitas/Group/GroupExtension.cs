namespace Entitas {

    public static class GroupExtension {

        /// <summary>
        /// Creates a Collector for this group.
        /// </summary>
        public static ICollector<TEntity> CreateCollector<TEntity>(this IGroup<TEntity> group, GroupEvent groupEvent = GroupEvent.Added) where TEntity : class, IEntity {
            return new Collector<TEntity>(group, groupEvent);
        }
    }
}
