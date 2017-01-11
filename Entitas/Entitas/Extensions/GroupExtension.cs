namespace Entitas {

    public static class GroupExtension {

        /// Creates an Collector for this group.
        public static Collector<TEntity> CreateCollector<TEntity>(
            this IGroup<TEntity> group,
            GroupEvent groupEvent = GroupEvent.Added)
            where TEntity : class, IEntity{
            return new Collector<TEntity>(group, groupEvent);
        }
    }
}
