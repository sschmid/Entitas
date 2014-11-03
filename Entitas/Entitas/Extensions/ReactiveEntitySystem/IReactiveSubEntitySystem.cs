namespace Entitas {
    public interface IReactiveSubEntitySystem {
        IEntityMatcher GetTriggeringMatcher();

        EntityCollectionEventType GetEventType();

        void Execute(Entity[] entities);
    }
}

