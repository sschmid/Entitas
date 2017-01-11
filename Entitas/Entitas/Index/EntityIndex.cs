using System;
using System.Collections.Generic;

namespace Entitas {

    public class EntityIndex<TEntity, TKey> : AbstractEntityIndex<TEntity, TKey> where TEntity : class, IEntity {

        readonly Dictionary<TKey, HashSet<TEntity>> _index;

        public EntityIndex(IGroup<TEntity> group, Func<TEntity, IComponent, TKey> getKey) :
            base(group, getKey) {
            _index = new Dictionary<TKey, HashSet<TEntity>>();
            Activate();
        }

        public EntityIndex(
            IGroup<TEntity> group,
            Func<TEntity, IComponent, TKey> getKey,
            IEqualityComparer<TKey> comparer) : base(group, getKey) {
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
                    entity.Release(this);
                }
            }

            _index.Clear();
        }

        protected override void addEntity(TEntity entity, IComponent component) {
            GetEntities(_getKey(entity, component)).Add(entity);
            entity.Retain(this);
        }

        protected override void removeEntity(
            TEntity entity, IComponent component) {
            GetEntities(_getKey(entity, component)).Remove(entity);
            entity.Release(this);
        }
    }
}
