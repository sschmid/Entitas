using System.Collections.Generic;

namespace Entitas {

    public delegate void ContextChanged<TEntity>(IContext<TEntity> context, IEntity entity) where TEntity : class, IEntity, new();
    public delegate void ContextGroupChanged<TEntity>(IContext<TEntity> context, IGroup<TEntity> group) where TEntity : class, IEntity, new();

    public interface IContext {

        int totalComponents { get; }

        Stack<IComponent>[] componentPools {get; }
        ContextInfo contextInfo { get; }

        int count { get; }
        int reusableEntitiesCount { get; }
        int retainedEntitiesCount { get; }

        void DestroyAllEntities();

        void ClearGroups();

        void AddEntityIndex(string name, IEntityIndex entityIndex);
        IEntityIndex GetEntityIndex(string name);

        void DeactivateAndRemoveEntityIndices();

        void ResetCreationIndex();
        void ClearComponentPool(int index);
        void ClearComponentPools();
        void Reset();
    }

    public interface IContext<TEntity> : IContext where TEntity : class, IEntity, new() {

        event ContextChanged<TEntity> OnEntityCreated;
        event ContextChanged<TEntity> OnEntityWillBeDestroyed;
        event ContextChanged<TEntity> OnEntityDestroyed;
        event ContextGroupChanged<TEntity> OnGroupCreated;
        event ContextGroupChanged<TEntity> OnGroupCleared;

        TEntity CreateEntity();
        void DestroyEntity(TEntity entity);
        bool HasEntity(TEntity entity);
        TEntity[] GetEntities();

        IGroup<TEntity> GetGroup(IMatcher<TEntity> matcher);
    }
}
