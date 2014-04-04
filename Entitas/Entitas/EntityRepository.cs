using ToolKit;
using System.Collections.Generic;
using System;

namespace Entitas {
    public class EntityRepository {
        readonly OrderedSet<Entity> _entities = new OrderedSet<Entity>();
        readonly Dictionary<IEntityMatcher, EntityCollection> _collections = new Dictionary<IEntityMatcher, EntityCollection>();
        readonly List<EntityCollection>[] _collectionsForIndex;
        readonly List<EntityCollection> _collectionList = new List<EntityCollection>();
        readonly int _numComponents;
        ulong _creationIndex;
        Entity[] _entitiesCache;

        public EntityRepository(int numComponents) {
            _numComponents = numComponents;
            _creationIndex = 0;
            _collectionsForIndex = new List<EntityCollection>[numComponents];
        }

        public EntityRepository(int numComponents, ulong startCreationIndex) {
            _numComponents = numComponents;
            _creationIndex = startCreationIndex;
            _collectionsForIndex = new List<EntityCollection>[numComponents];
        }

        public Entity CreateEntity() {
            var entity = new Entity(_numComponents, _creationIndex++);
            _entities.Add(entity);
            _entitiesCache = null;
            entity.OnComponentAdded += onComponentAdded;
            entity.OnComponentRemoved += onComponentRemoved;
            entity.OnComponentReplaced += onComponentReplaced;

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
                _collectionList.Add(collection);

                foreach (var index in matcher.indices) {
                    if (_collectionsForIndex[index] == null)
                        _collectionsForIndex[index] = new List<EntityCollection>();

                    _collectionsForIndex[index].Add(collection);
                }
            }

            return _collections[matcher];
        }

        void onComponentAdded(Entity entity, int index, IComponent component) {
            if (_collectionsForIndex[index] != null) {
                var collections = _collectionsForIndex[index];
                foreach (var collection in collections)
                    collection.AddEntityIfMatching(entity);
            }
        }

        void onComponentRemoved(Entity entity, int index, IComponent component) {
            if (_collectionsForIndex[index] != null) {
                var collections = _collectionsForIndex[index];
                foreach (var collection in collections)
                    collection.RemoveEntity(entity);
            }
        }

        void onComponentReplaced(Entity entity, int index, IComponent component) {
            if (_collectionsForIndex[index] != null) {
                var collections = _collectionsForIndex[index];
                foreach (var collection in collections)
                    collection.ReplaceEntity(entity);
            }
        }

        void removeFromAllCollections(Entity entity) {
            foreach (var collection in _collectionList)
                collection.RemoveEntity(entity);
        }
    }
}

