using System.Collections.Generic;

namespace Entitas {
    public enum EntityCollectionEventType : byte {
        None,
        OnEntityAdded,
        OnEntityRemoved
    }

    public interface IReactiveSubEntitySystem {
        IEntityMatcher GetTriggeringMatcher();

        EntityCollectionEventType GetEventType();

        void Execute(List<Entity> entities);
    }
}

