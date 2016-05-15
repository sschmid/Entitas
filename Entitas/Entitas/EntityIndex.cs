using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;

namespace Entitas {

    public interface IEntityIndex {
        void Deactivate();
    }

    public abstract class AbstractEntityIndex<T> : IEntityIndex {

        protected readonly Group _group;
        protected readonly Func<IComponent, T> _getKey;

        protected AbstractEntityIndex(Group group, Func<IComponent, T> getKey) {
            _group = group;
            _getKey = getKey;

            group.OnEntityAdded += onEntityAdded;
            group.OnEntityRemoved += onEntityRemoved;
        }

        public virtual void Deactivate() {
            _group.OnEntityAdded -= onEntityAdded;
            _group.OnEntityRemoved -= onEntityRemoved;
        }

        protected void indexEntities(Group group) {
            var entities = group.GetEntities();
            var index = group.matcher.indices.Single();
            for (int i = 0, entitiesLength = entities.Length; i < entitiesLength; i++) {
                var entity = entities[i];
                onEntityAdded(group, entity, index, entity.GetComponent(index));
            }
        }

        protected abstract void onEntityAdded(Group group, Entity entity, int index, IComponent component);

        protected abstract void onEntityRemoved(Group group, Entity entity, int index, IComponent component);
    }

    public class PrimaryEntityIndex<T> : AbstractEntityIndex<T> {

        readonly Dictionary<T, Entity> _index;

        public PrimaryEntityIndex(Group group, Func<IComponent, T> getKey) : base(group, getKey) {
            _index = new Dictionary<T, Entity>();
            indexEntities(group);
        }

        public bool HasEntity(T key) {
            return _index.ContainsKey(key);
        }

        public Entity GetEntity(T key) {
            var entity = TryGetEntity(key);
            if (entity == null) {
                throw new EntityIndexException("Entity for key '" + key + "' doesn't exist!",
                    "You should check if an entity with that key exists before getting it.");
            }

            return entity;
        }

        public Entity TryGetEntity(T key) {
            Entity entity;
            _index.TryGetValue(key, out entity);
            return entity;
        }

        public override void Deactivate() {
            base.Deactivate();

            foreach (var entity in _index.Values) {
                entity.Release(this);
            }

            _index.Clear();
        }

        protected override void onEntityAdded(Group group, Entity entity, int index, IComponent component) {
            var key = _getKey(component);
            if (_index.ContainsKey(key)) {
                throw new EntityIndexException("Entity for key '" + key + "' already exists!",
                    "Only one entity for a primary key is allowed.");
            }

            _index.Add(key, entity);
            entity.Retain(this);
        }

        protected override void onEntityRemoved(Group group, Entity entity, int index, IComponent component) {
            _index.Remove(_getKey(component));
            entity.Release(this);
        }
    }

    public class EntityIndexException : EntitasException {
        public EntityIndexException(string message, string hint) :
            base(message, hint) {
        }
    }
}