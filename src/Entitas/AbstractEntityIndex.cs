using System;

namespace Entitas
{
    public abstract class AbstractEntityIndex<TEntity, TKey> : IEntityIndex where TEntity : class, IEntity
    {
        public string Name => _name;

        protected readonly string _name;
        protected readonly IGroup<TEntity> _group;
        protected readonly Func<TEntity, IComponent, TKey> _getKey;
        protected readonly Func<TEntity, IComponent, TKey[]> _getKeys;
        protected readonly bool _isSingleKey;

        protected AbstractEntityIndex(string name, IGroup<TEntity> group, Func<TEntity, IComponent, TKey> getKey)
        {
            _name = name;
            _group = group;
            _getKey = getKey;
            _isSingleKey = true;
        }

        protected AbstractEntityIndex(string name, IGroup<TEntity> group, Func<TEntity, IComponent, TKey[]> getKeys)
        {
            _name = name;
            _group = group;
            _getKeys = getKeys;
            _isSingleKey = false;
        }

        public virtual void Activate()
        {
            _group.OnEntityAdded += OnEntityAdded;
            _group.OnEntityRemoved += OnEntityRemoved;
        }

        public virtual void Deactivate()
        {
            _group.OnEntityAdded -= OnEntityAdded;
            _group.OnEntityRemoved -= OnEntityRemoved;
            Clear();
        }

        protected void IndexEntities(IGroup<TEntity> group)
        {
            foreach (var entity in group)
                if (_isSingleKey)
                    AddEntity(_getKey(entity, null), entity);
                else
                    foreach (var key in _getKeys(entity, null))
                        AddEntity(key, entity);
        }

        protected void OnEntityAdded(IGroup<TEntity> group, TEntity entity, int index, IComponent component)
        {
            if (_isSingleKey)
                AddEntity(_getKey(entity, component), entity);
            else
                foreach (var key in _getKeys(entity, component))
                    AddEntity(key, entity);
        }

        protected void OnEntityRemoved(IGroup<TEntity> group, TEntity entity, int index, IComponent component)
        {
            if (_isSingleKey)
                RemoveEntity(_getKey(entity, component), entity);
            else
                foreach (var key in _getKeys(entity, component))
                    RemoveEntity(key, entity);
        }

        protected abstract void AddEntity(TKey key, TEntity entity);
        protected abstract void RemoveEntity(TKey key, TEntity entity);
        protected abstract void Clear();

        public override string ToString() => Name;

        ~AbstractEntityIndex() => Deactivate();
    }
}
