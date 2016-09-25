using System;
using System.Collections.Generic;

namespace Entitas {

    public interface IEntityIndex {
        void Activate();
        void Deactivate();
    }

    public abstract class AbstractEntityIndex<T> : IEntityIndex {

        protected readonly Group _group;
        protected readonly Func<Entity, IComponent, T> _getKey;

        protected AbstractEntityIndex(Group group, Func<Entity, IComponent, T> getKey) {
            _group = group;
            _getKey = getKey;
        }

        public virtual void Activate() {
            _group.OnEntityAdded += onEntityAdded;
            _group.OnEntityRemoved += onEntityRemoved;
        }

        public virtual void Deactivate() {
            _group.OnEntityAdded -= onEntityAdded;
            _group.OnEntityRemoved -= onEntityRemoved;
            clear();
        }

        protected void indexEntities(Group group) {
            var entities = group.GetEntities();
            for (int i = 0; i < entities.Length; i++) {
                addEntity(entities[i], null);
            }
        }

        protected void onEntityAdded(Group group, Entity entity, int index, IComponent component) {
            addEntity(entity, component);
        }

        protected void onEntityRemoved(Group group, Entity entity, int index, IComponent component) {
            removeEntity(entity, component);
        }

        protected abstract void addEntity(Entity entity, IComponent component);

        protected abstract void removeEntity(Entity entity, IComponent component);

        protected abstract void clear();

        ~AbstractEntityIndex () {
            Deactivate();
        }
    }

    public class PrimaryEntityIndex<T> : AbstractEntityIndex<T> {

        readonly Dictionary<T, Entity> _index;

        public PrimaryEntityIndex(Group group, Func<Entity, IComponent, T> getKey) : base(group, getKey) {
            _index = new Dictionary<T, Entity>();
            Activate();
        }

        public PrimaryEntityIndex(Group group, Func<Entity, IComponent, T> getKey, IEqualityComparer<T> comparer) : base(group, getKey) {
            _index = new Dictionary<T, Entity>(comparer);
            Activate();
        }

        public override void Activate() {
            base.Activate();
            indexEntities(_group);
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

        protected override void clear() {
            foreach (var entity in _index.Values) {
                entity.Release(this);
            }

            _index.Clear();
        }

        protected override void addEntity(Entity entity, IComponent component) {
            var key = _getKey(entity, component);
            if (_index.ContainsKey(key)) {
                throw new EntityIndexException("Entity for key '" + key + "' already exists!",
                    "Only one entity for a primary key is allowed.");
            }

            _index.Add(key, entity);
            entity.Retain(this);
        }

        protected override void removeEntity(Entity entity, IComponent component) {
            _index.Remove(_getKey(entity, component));
            entity.Release(this);
        }
    }

    public class EntityIndex<T> : AbstractEntityIndex<T> {

        readonly Dictionary<T, HashSet<Entity>> _index;

        public EntityIndex(Group group, Func<Entity, IComponent, T> getKey) : base(group, getKey) {
            _index = new Dictionary<T, HashSet<Entity>>();
            Activate();
        }

        public EntityIndex(Group group, Func<Entity, IComponent, T> getKey, IEqualityComparer<T> comparer) : base(group, getKey) {
            _index = new Dictionary<T, HashSet<Entity>>(comparer);
            Activate();
        }

        public override void Activate() {
            base.Activate();
            indexEntities(_group);
        }

        public HashSet<Entity> GetEntities(T key) {
            HashSet<Entity> entities;
            if (!_index.TryGetValue(key, out entities)) {
                entities = new HashSet<Entity>(EntityEqualityComparer.comparer);
                _index.Add(key, entities);
            }

            return entities;
        }

        protected override void clear() {
            foreach (var entities in _index.Values) {
                foreach (var entity in entities) {
                    entity.Release(this);
                }
            }

            _index.Clear();
        }

        protected override void addEntity(Entity entity, IComponent component) {
            GetEntities(_getKey(entity, component)).Add(entity);
            entity.Retain(this);
        }

        protected override void removeEntity(Entity entity, IComponent component) {
            GetEntities(_getKey(entity, component)).Remove(entity);
            entity.Release(this);
        }
    }

    public class EntityIndexException : EntitasException {
        public EntityIndexException(string message, string hint) :
            base(message, hint) {
        }
    }
}
