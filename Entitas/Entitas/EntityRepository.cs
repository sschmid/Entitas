using ToolKit;
using System.Collections.Generic;
using System.Linq;

namespace Entitas {
    public class EntityRepository {
        readonly OrderedSet<Entity> _entities = new OrderedSet<Entity>();
        readonly Dictionary<IEntityMatcher, EntityCollection> _collections = new Dictionary<IEntityMatcher, EntityCollection>();
        ulong _creationIndex;
        Entity[] _entitiesCache;
        EntityCollection[] _collectionCache = new EntityCollection[0];

        public EntityRepository() {
            _creationIndex = 0;
        }

        public EntityRepository(ulong startCreationIndex) {
            _creationIndex = startCreationIndex;
        }

        public Entity CreateEntity() {
            var entity = new Entity(_creationIndex++);
            entity.OnComponentAdded += onComponentAdded;
            entity.OnComponentRemoved += onComponentRemoved;
            entity.OnComponentReplaced += onComponentReplaced;
            _entities.Add(entity);
            _entitiesCache = null;
            return entity;
        }

        public void DestroyEntity(Entity entity) {
            entity.OnComponentAdded -= onComponentAdded;
            entity.OnComponentRemoved -= onComponentRemoved;
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
                    collection.HandleEntity(e);
                _collections.Add(matcher, collection);
                _collectionCache = _collections.Values.ToArray();
            }

            return _collections[matcher];
        }

        void onComponentAdded(Entity entity, IComponent component) {
            foreach (var collection in _collectionCache)
                collection.HandleEntity(entity);
        }

        void onComponentRemoved(Entity entity, IComponent component) {
            foreach (var collection in _collectionCache)
                collection.HandleEntity(entity);
        }

        void onComponentReplaced(Entity entity, IComponent component) {
            foreach (var collection in _collectionCache)
                if (collection.matcher.HasType(component.GetType()))
                    collection.ReplaceEntity(entity);
        }

        void removeFromAllCollections(Entity entity) {
            foreach (var collection in _collectionCache)
                collection.RemoveEntity(entity);
        }
    }
}

