using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;

namespace Entitas {

    public interface IEntityIndex {
        void Deactivate();
    }

    public class EntityIndex<T> : IEntityIndex {

        readonly Group _group;
        readonly Func<IComponent, T> _getKey;
        readonly Dictionary<T, Entity> _index;

        public EntityIndex(Group group, Func<IComponent, T> getKey) {
            _group = group;
            _getKey = getKey;
            _index = new Dictionary<T, Entity>();

            var entities = group.GetEntities();
            var index = group.matcher.indices.Single();
            for (int i = 0, entitiesLength = entities.Length; i < entitiesLength; i++) {
                var entity = entities[i];
                onEntityAdded(group, entity, index, entity.GetComponent(index));
            }

            group.OnEntityAdded += onEntityAdded;
            group.OnEntityRemoved += onEntityRemoved;
        }

        public bool HasEntity(T key) {
            return _index.ContainsKey(key);
        }

        public Entity GetEntity(T key) {
            var entity = TryGetEntity(key);
            if (entity == null) {
                throw new EntityIndexException("Entity with key '" + key + "' doesn't exist!");
            }

            return entity;
        }

        public Entity TryGetEntity(T key) {
            Entity entity;
            _index.TryGetValue(key, out entity);
            return entity;
        }

        public void Deactivate() {
            _group.OnEntityAdded -= onEntityAdded;
            _group.OnEntityRemoved -= onEntityRemoved;

            foreach (var entity in _index.Values) {
                entity.Release(this);
            }

            _index.Clear();
        }

        void onEntityAdded(Group group, Entity entity, int index, IComponent component) {
            _index.Add(_getKey(component), entity);
            entity.Retain(this);
        }

        void onEntityRemoved(Group group, Entity entity, int index, IComponent component) {
            _index.Remove(_getKey(component));
            entity.Release(this);
        }
    }

    public class EntityIndexException : EntitasException {
        public EntityIndexException(string message) :
            base(message, "You should check if an entity with that key exists before getting it.") {
        }
    }
}