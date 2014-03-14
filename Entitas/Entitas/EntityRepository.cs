using ToolKit;
using System.Collections.Generic;

namespace Entitas {
    public class EntityRepository {
        readonly OrderedSet<Entity> _entities = new OrderedSet<Entity>();
        readonly Dictionary<IEntityMatcher, EntityCollection> _collections = new Dictionary<IEntityMatcher, EntityCollection>();
        ulong _creationIndex;
        Entity[] _entitiesCache;

        public EntityRepository() {
            _creationIndex = 0;
        }

        public EntityRepository(ulong startCreationIndex) {
            _creationIndex = startCreationIndex;
        }

        public Entity CreateEntity() {
            var entity = new Entity(_creationIndex++);
            entity.OnComponentAdded += onEntityAdded;
            entity.OnComponentRemoved += onEntityRemoved;
            _entities.Add(entity);
            _entitiesCache = null;
            return entity;
        }

        public void DestroyEntity(Entity entity) {
            entity.OnComponentAdded -= onEntityAdded;
            entity.OnComponentRemoved -= onEntityRemoved;
            entity.RemoveAllComponents();
            removeFromAllCollections(entity);
            _entities.Remove(entity);
            _entitiesCache = null;
        }

        public void DestroyAllEntities() {
            var entities = GetEntities();
            foreach (var e in entities)
                DestroyEntity(e);
        }

        public bool HasEntity(Entity entity) {
            return _entities.Contains(entity);
        }

        public Entity[] GetEntities() {
            if (_entitiesCache == null)
                _entitiesCache = _entities.ToArray();

            return _entitiesCache;
        }

        public EntityCollection GetCollection(IEntityMatcher matcher) {
            if (!_collections.ContainsKey(matcher)) {
                var collection = new EntityCollection(matcher);
                foreach (var e in GetEntities())
                    collection.AddEntityIfMatching(e);
                _collections.Add(matcher, collection);
            }

            return _collections[matcher];
        }

        void onEntityAdded(Entity entity, IComponent component) {
            var collections = _collections.Values;
            foreach (var collection in collections)
                collection.AddEntityIfMatching(entity);
        }

        void onEntityRemoved(Entity entity, IComponent component) {
            var collections = _collections.Values;
            foreach (var collection in collections)
                collection.RemoveEntityIfNotMatching(entity);
        }

        void removeFromAllCollections(Entity entity) {
            var collections = _collections.Values;
            foreach (var collection in collections)
                collection.RemoveEntity(entity);
        }
    }
}

