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

        public override string ToString() => Name;

        protected void IndexEntities(IGroup<TEntity> group)
        {
            foreach (var entity in group)
            {
                if (_isSingleKey)
                {
                    AddEntity(_getKey(entity, null), entity);
                }
                else
                {
                    var keys = _getKeys(entity, null);
                    for (var i = 0; i < keys.Length; i++)
                        AddEntity(keys[i], entity);
                }
            }
        }

        protected void OnEntityAdded(IGroup<TEntity> group, TEntity entity, int index, IComponent component)
        {
            if (_isSingleKey)
            {
                AddEntity(_getKey(entity, component), entity);
            }
            else
            {
                var keys = _getKeys(entity, component);
                for (var i = 0; i < keys.Length; i++)
                    AddEntity(keys[i], entity);
            }
        }

        protected void OnEntityRemoved(IGroup<TEntity> group, TEntity entity, int index, IComponent component)
        {
            if (_isSingleKey)
            {
                RemoveEntity(_getKey(entity, component), entity);
            }
            else
            {
                var keys = _getKeys(entity, component);
                for (var i = 0; i < keys.Length; i++)
                    RemoveEntity(keys[i], entity);
            }
        }

        protected abstract void AddEntity(TKey key, TEntity entity);

        protected abstract void RemoveEntity(TKey key, TEntity entity);

        protected abstract void Clear();

        ~AbstractEntityIndex() => Deactivate();
    }
}
