namespace Entitas {
    public enum EntityCollectionEventType : byte {
        None,
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

