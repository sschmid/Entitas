namespace Entitas {
    public enum EntityCollectionEventType {
        OnEntityAdded,
        OnEntityRemoved,
        OnEntityAddedSafe
    }

    public interface IReactiveSubEntitySystem {
        IEntityMatcher GetTriggeringMatcher();

        EntityCollectionEventType GetEventType();

        void Execute(Entity[] entities);
    }
}

