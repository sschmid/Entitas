using System;
using System.Collections.Generic;

namespace Entitas {

    public class PrimaryEntityIndex<T> : AbstractEntityIndex<T> {

        readonly Dictionary<T, IEntity> _index;

        public PrimaryEntityIndex(
            Group group,
            Func<IEntity, IComponent, T> getKey
        ) : base(group, getKey) {
            _index = new Dictionary<T, IEntity>();
            Activate();
        }

        public PrimaryEntityIndex(
            Group group, Func<IEntity,
            IComponent, T> getKey,
            IEqualityComparer<T> comparer) : base(group, getKey) {
            _index = new Dictionary<T, IEntity>(comparer);
            Activate();
        }

        public override void Activate() {
            base.Activate();
            indexEntities(_group);
        }

        public bool HasEntity(T key) {
            return _index.ContainsKey(key);
        }

        public IEntity GetEntity(T key) {
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

        public IEntity TryGetEntity(T key) {
            IEntity entity;
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
            IEntity entity, IComponent component) {
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
            IEntity entity, IComponent component) {
            _index.Remove(_getKey(entity, component));
            entity.Release(this);
        }
    }
}
