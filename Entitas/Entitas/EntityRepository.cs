using ToolKit;
using System.Collections.Generic;
using System;

namespace Entitas {
    public class EntityRepository {
        readonly OrderedSet<Entity> _entities = new OrderedSet<Entity>();
        readonly Dictionary<IEntityMatcher, EntityCollection> _collections = new Dictionary<IEntityMatcher, EntityCollection>();
        readonly Dictionary<Type, List<EntityCollection>> _collectionsForType = new Dictionary<Type, List<EntityCollection>>();
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
            entity.OnComponentReplaced -= onComponentReplaced;
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

                foreach (var type in matcher.types) {
                    if (!_collectionsForType.ContainsKey(type))
                        _collectionsForType.Add(type, new List<EntityCollection>());

                    _collectionsForType[type].Add(collection);
                }
            }

            return _collections[matcher];
        }

        void onComponentAdded(Entity entity, IComponent component) {
            var type = component.GetType();
            if (_collectionsForType.ContainsKey(type)) {
                var collections = _collectionsForType[type];
                foreach (var collection in collections)
                    collection.AddEntityIfMatching(entity);
            }
        }

        void onComponentRemoved(Entity entity, IComponent component) {
            var type = component.GetType();
            if (_collectionsForType.ContainsKey(type)) {
                var collections = _collectionsForType[type];
                foreach (var collection in collections)
                    collection.RemoveEntity(entity);
            }
        }

        void onComponentReplaced(Entity entity, IComponent component) {
            var type = component.GetType();
            if (_collectionsForType.ContainsKey(type)) {
                var collections = _collectionsForType[type];
                foreach (var collection in collections)
                    collection.ReplaceEntity(entity);
            }
        }

        void removeFromAllCollections(Entity entity) {
            var collections = _collections.Values;
            foreach (var collection in collections)
                collection.RemoveEntity(entity);
        }
    }
}

