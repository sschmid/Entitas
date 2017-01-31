using System;

namespace Entitas {

    public abstract class AbstractEntityIndex<TEntity, TKey> : IEntityIndex where TEntity : class, IEntity, new() {

        protected readonly IGroup<TEntity> _group;
        protected readonly Func<TEntity, IComponent, TKey> _getKey;

        protected AbstractEntityIndex(IGroup<TEntity> group, Func<TEntity, IComponent, TKey> getKey) {
            _group = group;
            _getKey = getKey;
        }

        public virtual void Activate() {
            _group.OnEntityAdded += onEntityAdded;
            _group.OnEntityRemoved += onEntityRemoved;
        }

        public virtual void Deactivate() {
            _group.OnEntityAdded -= onEntityAdded;
            _group.OnEntityRemoved -= onEntityRemoved;
            clear();
        }

        protected void indexEntities(IGroup<TEntity> group) {
            var entities = group.GetEntities();
            for(int i = 0; i < entities.Length; i++) {
                addEntity(entities[i], null);
            }
        }

        protected void onEntityAdded(
            IGroup<TEntity> group, TEntity entity, int index, IComponent component) {
            addEntity(entity, component);
        }

        protected void onEntityRemoved(
            IGroup<TEntity> group, TEntity entity, int index, IComponent component) {
            removeEntity(entity, component);
        }

        protected abstract void addEntity(TEntity entity, IComponent component);

        protected abstract void removeEntity(
            TEntity entity, IComponent component
        );

        protected abstract void clear();

        ~AbstractEntityIndex() {
            Deactivate();
        }
    }
}
