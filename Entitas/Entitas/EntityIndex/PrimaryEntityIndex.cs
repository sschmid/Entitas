using System;
using System.Collections.Generic;

namespace Entitas {

    public class PrimaryEntityIndex<TEntity, TKey> : AbstractEntityIndex<TEntity, TKey> where TEntity : class, IEntity, new() {

        readonly Dictionary<TKey, TEntity> _index;

        public PrimaryEntityIndex(IGroup<TEntity> group, Func<TEntity, IComponent, TKey> getKey)
            : base(group, getKey) {
            _index = new Dictionary<TKey, TEntity>();
            Activate();
        }

        public PrimaryEntityIndex(IGroup<TEntity> group, Func<TEntity, IComponent, TKey> getKey, IEqualityComparer<TKey> comparer)
            : base(group, getKey) {
            _index = new Dictionary<TKey, TEntity>(comparer);
            Activate();
        }

        public override void Activate() {
            base.Activate();
            indexEntities(_group);
        }

        public bool HasEntity(TKey key) {
            return _index.ContainsKey(key);
        }

        public TEntity GetEntity(TKey key) {
            var entity = TryGetEntity(key);
            if(entity == null) {
                throw new EntityIndexException(
                    "Entity for key '" + key + "' doesn't exist!",
                    "You should check if an entity with that key exists " +
                    "before getting it."
                );
            }

            return entity;
        }

        public TEntity TryGetEntity(TKey key) {
            TEntity entity;
            _index.TryGetValue(key, out entity);
            return entity;
        }

        protected override void clear() {
            foreach(var entity in _index.Values) {
                entity.Release(this);
            }

            _index.Clear();
        }

        protected override void addEntity(
            TEntity entity, IComponent component) {
            var key = _getKey(entity, component);
            if(_index.ContainsKey(key)) {
                throw new EntityIndexException(
                    "Entity for key '" + key + "' already exists!",
                    "Only one entity for a primary key is allowed.");
            }

            _index.Add(key, entity);
            entity.Retain(this);
        }

        protected override void removeEntity(
            TEntity entity, IComponent component) {
            _index.Remove(_getKey(entity, component));
            entity.Release(this);
        }
    }
}
