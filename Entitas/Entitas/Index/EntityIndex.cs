using System;
using System.Collections.Generic;

namespace Entitas {

    public class EntityIndex<T> : AbstractEntityIndex<T> {

        readonly Dictionary<T, HashSet<Entity>> _index;

        public EntityIndex(Group group, Func<Entity, IComponent, T> getKey) :
            base(group, getKey) {
            _index = new Dictionary<T, HashSet<Entity>>();
            Activate();
        }

        public EntityIndex(
            Group group,
            Func<Entity, IComponent, T> getKey,
            IEqualityComparer<T> comparer) : base(group, getKey) {
            _index = new Dictionary<T, HashSet<Entity>>(comparer);
            Activate();
        }

        public override void Activate() {
            base.Activate();
            indexEntities(_group);
        }

        public HashSet<Entity> GetEntities(T key) {
            HashSet<Entity> entities;
            if(!_index.TryGetValue(key, out entities)) {
                entities = new HashSet<Entity>(EntityEqualityComparer.comparer);
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

        protected override void addEntity(Entity entity, IComponent component) {
            GetEntities(_getKey(entity, component)).Add(entity);
            entity.Retain(this);
        }

        protected override void removeEntity(
            Entity entity, IComponent component) {
            GetEntities(_getKey(entity, component)).Remove(entity);
            entity.Release(this);
        }
    }
}
