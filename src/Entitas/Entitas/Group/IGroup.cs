using System.Collections.Generic;

namespace Entitas {

    public delegate void GroupChanged<TEntity>(
        IGroup<TEntity> group, TEntity entity, int index, IComponent component
    ) where TEntity : class, IEntity;

    public delegate void GroupUpdated<TEntity>(
        IGroup<TEntity> group, TEntity entity, int index,
        IComponent previousComponent, IComponent newComponent
    ) where TEntity : class, IEntity;

    public interface IGroup {

        int count { get; }

        void RemoveAllEventHandlers();
    }

    public interface IGroup<TEntity> : IGroup where TEntity : class, IEntity {

        event GroupChanged<TEntity> OnEntityAdded;
        event GroupChanged<TEntity> OnEntityRemoved;
        event GroupUpdated<TEntity> OnEntityUpdated;

        IMatcher<TEntity> matcher { get; }

        void HandleEntitySilently(TEntity entity);
        void HandleEntity(TEntity entity, int index, IComponent component);
        GroupChanged<TEntity> HandleEntity(TEntity entity);

        void UpdateEntity(TEntity entity, int index, IComponent previousComponent, IComponent newComponent);

        bool ContainsEntity(TEntity entity);

        TEntity[] GetEntities();
        List<TEntity> GetEntities(List<TEntity> buffer);
        TEntity GetSingleEntity();

        IEnumerable<TEntity> AsEnumerable();
        HashSet<TEntity>.Enumerator GetEnumerator();
    }
}
