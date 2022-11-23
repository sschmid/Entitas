namespace Entitas
{
    public static class CollectorContextExtension
    {
        public static ICollector<TEntity> CreateCollector<TEntity>(this IContext<TEntity> context, IMatcher<TEntity> matcher)
            where TEntity : class, IEntity =>
            context.CreateCollector(new TriggerOnEvent<TEntity>(matcher, GroupEvent.Added));

        public static ICollector<TEntity> CreateCollector<TEntity>(this IContext<TEntity> context, params TriggerOnEvent<TEntity>[] triggers)
            where TEntity : class, IEntity
        {
            var groups = new IGroup<TEntity>[triggers.Length];
            var groupEvents = new GroupEvent[triggers.Length];

            for (var i = 0; i < triggers.Length; i++)
            {
                groups[i] = context.GetGroup(triggers[i].Matcher);
                groupEvents[i] = triggers[i].GroupEvent;
            }

            return new Collector<TEntity>(groups, groupEvents);
        }
    }
}
