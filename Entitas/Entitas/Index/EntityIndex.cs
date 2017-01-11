using System;
using System.Collections.Generic;

namespace Entitas {

    public class EntityIndex<T> : AbstractEntityIndex<T> {

        readonly Dictionary<T, HashSet<IEntity>> _index;

        public EntityIndex(Group group, Func<IEntity, IComponent, T> getKey) :
            base(group, getKey) {
            _index = new Dictionary<T, HashSet<IEntity>>();
            Activate();
        }

        public EntityIndex(
            Group group,
            Func<IEntity, IComponent, T> getKey,
            IEqualityComparer<T> comparer) : base(group, getKey) {
            _index = new Dictionary<T, HashSet<IEntity>>(comparer);
            Activate();
        }

        public override void Activate() {
            base.Activate();
            indexEntities(_group);
        }

        public HashSet<IEntity> GetEntities(T key) {
            HashSet<IEntity> entities;
            if(!_index.TryGetValue(key, out entities)) {
                entities = new HashSet<IEntity>(EntityEqualityComparer.comparer);
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

        protected override void addEntity(IEntity entity, IComponent component) {
            GetEntities(_getKey(entity, component)).Add(entity);
            entity.Retain(this);
        }

        protected override void removeEntity(
            IEntity entity, IComponent component) {
            GetEntities(_getKey(entity, component)).Remove(entity);
            entity.Release(this);
        }
    }
}
