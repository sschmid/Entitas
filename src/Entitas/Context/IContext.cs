using System.Collections.Generic;

namespace Entitas
{
    public delegate void ContextEntityChanged(IContext context, Entity entity);

    public delegate void ContextGroupChanged(IContext context, IGroup group);

    public interface IContext
    {
        event ContextEntityChanged OnEntityCreated;
        event ContextEntityChanged OnEntityWillBeDestroyed;
        event ContextEntityChanged OnEntityDestroyed;

        event ContextGroupChanged OnGroupCreated;

        int TotalComponents { get; }

        Stack<IComponent>[] ComponentPools { get; }
        ContextInfo ContextInfo { get; }

        int Count { get; }
        int ReusableEntitiesCount { get; }
        int RetainedEntitiesCount { get; }

        void DestroyAllEntities();

        void AddEntityIndex(IEntityIndex entityIndex);
        IEntityIndex GetEntityIndex(string name);

        void ResetCreationIndex();
        void ClearComponentPool(int index);
        void ClearComponentPools();
        void RemoveAllEventHandlers();
        void Reset();
    }

    public interface IContext<TEntity> : IContext where TEntity : Entity
    {
        TEntity CreateEntity();

        bool HasEntity(TEntity entity);
        TEntity[] GetEntities();
        TEntity[] GetEntities(IMatcher<TEntity> matcher);
        IGroup<TEntity> GetGroup(IMatcher<TEntity> matcher);
    }
}
