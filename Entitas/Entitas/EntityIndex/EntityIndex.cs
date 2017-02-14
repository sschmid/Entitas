using System;
using System.Collections.Generic;

namespace Entitas {

    public class EntityIndex<TEntity, TKey> : AbstractEntityIndex<TEntity, TKey> where TEntity : class, IEntity, new() {

        readonly Dictionary<TKey, HashSet<TEntity>> _index;

        public EntityIndex(IGroup<TEntity> group, Func<TEntity, IComponent, TKey> getKey) : base(group, getKey) {
            _index = new Dictionary<TKey, HashSet<TEntity>>();
            Activate();
        }

        public EntityIndex(IGroup<TEntity> group, Func<TEntity, IComponent, TKey[]> getKeys) : base(group, getKeys) {
            _index = new Dictionary<TKey, HashSet<TEntity>>();
            Activate();
        }

        public EntityIndex(IGroup<TEntity> group, Func<TEntity, IComponent, TKey> getKey, IEqualityComparer<TKey> comparer) : base(group, getKey) {
            _index = new Dictionary<TKey, HashSet<TEntity>>(comparer);
            Activate();
        }

        public EntityIndex(IGroup<TEntity> group, Func<TEntity, IComponent, TKey[]> getKeys, IEqualityComparer<TKey> comparer) : base(group, getKeys) {
            _index = new Dictionary<TKey, HashSet<TEntity>>(comparer);
            Activate();
        }

        public override void Activate() {
            base.Activate();
            indexEntities(_group);
        }

        public HashSet<TEntity> GetEntities(TKey key) {
            HashSet<TEntity> entities;
            if(!_index.TryGetValue(key, out entities)) {
                entities = new HashSet<TEntity>(EntityEqualityComparer<TEntity>.comparer);
                _index.Add(key, entities);
            }

            return entities;
        }

        protected override void clear() {
            foreach(var entities in _index.Values) {
                foreach(var entity in entities) {

#if ENTITAS_FAST_AND_UNSAFE
                    entity.Release(this);
#else
                    if(entity.owners.Contains(this)) {
                        entity.Release(this);
                    }
#endif
                }
            }

            _index.Clear();
        }

        protected override void addEntity(TKey key, TEntity entity) {
            GetEntities(key).Add(entity);

#if ENTITAS_FAST_AND_UNSAFE
            entity.Retain(this);
#else
            if(!entity.owners.Contains(this)) {
                entity.Retain(this);
            }
#endif
        }

        protected override void removeEntity(TKey key, TEntity entity) {
            GetEntities(key).Remove(entity);

#if ENTITAS_FAST_AND_UNSAFE
            entity.Release(this);
#else
            if(entity.owners.Contains(this)) {
                entity.Release(this);
            }
#endif
        }
    }
}
