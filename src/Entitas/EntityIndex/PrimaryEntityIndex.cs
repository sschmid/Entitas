using System;
using System.Collections.Generic;

namespace Entitas
{
    public class PrimaryEntityIndex<TEntity, TKey> : AbstractEntityIndex<TEntity, TKey> where TEntity : class, IEntity
    {
        readonly Dictionary<TKey, TEntity> _index;

        public PrimaryEntityIndex(string name, IGroup<TEntity> group, Func<TEntity, IComponent, TKey> getKey) : base(name, group, getKey)
        {
            _index = new Dictionary<TKey, TEntity>();
            Activate();
        }

        public PrimaryEntityIndex(string name, IGroup<TEntity> group, Func<TEntity, IComponent, TKey[]> getKeys) : base(name, group, getKeys)
        {
            _index = new Dictionary<TKey, TEntity>();
            Activate();
        }

        public PrimaryEntityIndex(string name, IGroup<TEntity> group, Func<TEntity, IComponent, TKey> getKey, IEqualityComparer<TKey> comparer) : base(name, group, getKey)
        {
            _index = new Dictionary<TKey, TEntity>(comparer);
            Activate();
        }

        public PrimaryEntityIndex(string name, IGroup<TEntity> group, Func<TEntity, IComponent, TKey[]> getKeys, IEqualityComparer<TKey> comparer) : base(name, group, getKeys)
        {
            _index = new Dictionary<TKey, TEntity>(comparer);
            Activate();
        }

        public override void Activate()
        {
            base.Activate();
            indexEntities(_group);
        }

        public TEntity GetEntity(TKey key)
        {
            _index.TryGetValue(key, out var entity);
            return entity;
        }

        public override string ToString() => $"PrimaryEntityIndex({name})";

        protected override void clear()
        {
            foreach (var entity in _index.Values)
            {
                if (entity.aerc is SafeAERC safeAerc)
                {
                    if (safeAerc.owners.Contains(this))
                        entity.Release(this);
                }
                else
                {
                    entity.Release(this);
                }
            }

            _index.Clear();
        }

        protected override void addEntity(TKey key, TEntity entity)
        {
            if (_index.ContainsKey(key))
                throw new EntityIndexException(
                    $"Entity for key '{key}' already exists!",
                    "Only one entity for a primary key is allowed.");

            _index.Add(key, entity);

            if (entity.aerc is SafeAERC safeAerc)
            {
                if (!safeAerc.owners.Contains(this))
                    entity.Retain(this);
            }
            else
            {
                entity.Retain(this);
            }
        }

        protected override void removeEntity(TKey key, TEntity entity)
        {
            _index.Remove(key);

            if (entity.aerc is SafeAERC safeAerc)
            {
                if (safeAerc.owners.Contains(this))
                    entity.Release(this);
            }
            else
            {
                entity.Release(this);
            }
        }
    }
}
