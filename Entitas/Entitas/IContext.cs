using System.Collections.Generic;

namespace Entitas {

    public delegate void ContextChanged(Context context, IEntity entity);
    public delegate void GroupChanged(Context context, Group group);

    public interface IContext {

        event ContextChanged OnEntityCreated;
        event ContextChanged OnEntityWillBeDestroyed;
        event ContextChanged OnEntityDestroyed;
        event GroupChanged OnGroupCreated;
        event GroupChanged OnGroupCleared;

        int totalComponents { get; }

        Stack<IComponent>[] componentPools {get; }
        ContextInfo contextInfo { get; }

        int count { get; }
        int reusableEntitiesCount { get; }
        int retainedEntitiesCount { get; }

        IEntity CreateEntity();
        void DestroyEntity(IEntity entity);
        void DestroyAllEntities();
        bool HasEntity(IEntity entity);
        IEntity[] GetEntities();

        Group GetGroup(IMatcher matcher);
        void ClearGroups();

        void AddEntityIndex(string name, IEntityIndex entityIndex);
        IEntityIndex GetEntityIndex(string name);
        void DeactivateAndRemoveEntityIndices();

        void ResetCreationIndex();
        void ClearComponentPool(int index);
        void ClearComponentPools();
        void Reset();
    }
}
