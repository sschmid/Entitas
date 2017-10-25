using System;

namespace Entitas {

    public abstract class AbstractEntityIndex<TEntity, TKey> : IEntityIndex where TEntity : class, IEntity {

        public string name { get { return _name; } }

        protected readonly string _name;
        protected readonly IGroup<TEntity> _group;
        protected readonly Func<TEntity, IComponent, TKey> _getKey;
        protected readonly Func<TEntity, IComponent, TKey[]> _getKeys;
        protected readonly bool _isSingleKey;

        protected AbstractEntityIndex(string name, IGroup<TEntity> group, Func<TEntity, IComponent, TKey> getKey) {
            _name = name;
            _group = group;
            _getKey = getKey;
            _isSingleKey = true;
        }

        protected AbstractEntityIndex(string name, IGroup<TEntity> group, Func<TEntity, IComponent, TKey[]> getKeys) {
            _name = name;
            _group = group;
            _getKeys = getKeys;
            _isSingleKey = false;
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

        public override string ToString() {
            return name;
        }

        protected void indexEntities(IGroup<TEntity> group) {
            foreach (var entity in group) {
                if (_isSingleKey) {
                    addEntity(_getKey(entity, null), entity);
                } else {
                    var keys = _getKeys(entity, null);
                    for (int i = 0; i < keys.Length; i++) {
                        addEntity(keys[i], entity);
                    }
                }
            }
        }

        protected void onEntityAdded(IGroup<TEntity> group, TEntity entity, int index, IComponent component) {
            if (_isSingleKey) {
                addEntity(_getKey(entity, component), entity);
            } else {
                var keys = _getKeys(entity, component);
                for (int i = 0; i < keys.Length; i++) {
                    addEntity(keys[i], entity);
                }
            }
        }

        protected void onEntityRemoved(IGroup<TEntity> group, TEntity entity, int index, IComponent component) {
            if (_isSingleKey) {
                removeEntity(_getKey(entity, component), entity);
            } else {
                var keys = _getKeys(entity, component);
                for (int i = 0; i < keys.Length; i++) {
                    removeEntity(keys[i], entity);
                }
            }
        }

        protected abstract void addEntity(TKey key, TEntity entity);

        protected abstract void removeEntity(TKey key, TEntity entity);

        protected abstract void clear();

        ~AbstractEntityIndex() {
            Deactivate();
        }
    }
}
