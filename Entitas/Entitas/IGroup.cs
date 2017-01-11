namespace Entitas {

    public delegate void GroupChanged<TEntitiy>(
        IGroup<TEntitiy> group, TEntitiy entity, int index, IComponent component
    ) where TEntitiy : IEntity;

    public delegate void GroupUpdated<TEntitiy>(
        IGroup<TEntitiy> group, TEntitiy entity, int index,
        IComponent previousComponent, IComponent newComponent
    ) where TEntitiy : IEntity;

    public interface IGroup {

        int count { get; }
        IMatcher matcher { get; }

        void RemoveAllEventHandlers();
    }

    public interface IGroup<TEntity> : IGroup where TEntity : IEntity {

        event GroupChanged<TEntity> OnEntityAdded;
        event GroupChanged<TEntity> OnEntityRemoved;
        event GroupUpdated<TEntity> OnEntityUpdated;

        void HandleEntitySilently(TEntity entity);
        void HandleEntity(TEntity entity, int index, IComponent component);

        // TODO
        GroupChanged<TEntity> handleEntity(TEntity entity);

        void UpdateEntity(TEntity entity, int index, IComponent previousComponent, IComponent newComponent);

        bool ContainsEntity(TEntity entity);

        TEntity[] GetEntities();
        TEntity GetSingleEntity();
    }
}
