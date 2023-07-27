namespace Entitas
{
    public struct TriggerOnEvent<TEntity> where TEntity : Entity
    {
        public readonly IMatcher<TEntity> Matcher;
        public readonly GroupEvent GroupEvent;

        public TriggerOnEvent(IMatcher<TEntity> matcher, GroupEvent groupEvent)
        {
            Matcher = matcher;
            GroupEvent = groupEvent;
        }
    }
}
