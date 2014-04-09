using ToolKit;
using System.Collections.Generic;
using System;

namespace Entitas {
    public class EntityRepository {
        public int poolItemCount { get { return _entityPool.Count; } }

        readonly LinkedListSet<Entity> _entities = new LinkedListSet<Entity>();
        readonly Dictionary<IEntityMatcher, EntityCollection> _collections = new Dictionary<IEntityMatcher, EntityCollection>();
        readonly List<EntityCollection>[] _collectionsForIndex;
        readonly List<EntityCollection> _collectionList = new List<EntityCollection>();
        readonly ObjectPool<Entity> _entityPool;
        readonly int _totalComponents;
        ulong _creationIndex;
        Entity[] _entitiesCache;

        public EntityRepository(int totalComponents) : this(totalComponents, 0) {
        }

        public EntityRepository(int totalComponents, ulong startCreationIndex) {
            _totalComponents = totalComponents;
            _creationIndex = startCreationIndex;
            _collectionsForIndex = new List<EntityCollection>[totalComponents];
            _entityPool = new ObjectPool<Entity>(CreateEntity);
        }

        public Entity CreateEntity() {
            return setupEntity(new Entity(_totalComponents, _creationIndex++));
        }

        public Entity GetEntityFromPool() {
            return setupEntity(_entityPool.Get());
        }

        Entity setupEntity(Entity entity) {
            _entities.Add(entity);
            _entitiesCache = null;
            entity.OnComponentAdded += onComponentAdded;
            entity.OnComponentRemoved += onComponentRemoved;
            entity.OnComponentReplaced += onComponentReplaced;
            return entity;
        }

        public void PushToPool(Entity entity) {
            DestroyEntity(entity);
            _entityPool.Push(entity);
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
            for (int i = 0, entitiesLength = entities.Length; i < entitiesLength; i++)
                DestroyEntity(entities[i]);
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
                var entities = GetEntities();
                for (int i = 0, entitiesLength = entities.Length; i < entitiesLength; i++)
                    collection.AddEntityIfMatching(entities[i]);
                _collections.Add(matcher, collection);
                _collectionList.Add(collection);

                for (int i = 0, indicesLength = matcher.indices.Length; i < indicesLength; i++) {
                    var index = matcher.indices[i];
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
                for (int i = 0, collectionsCount = collections.Count; i < collectionsCount; i++)
                    collections[i].AddEntityIfMatching(entity);
            }
        }

        void onComponentRemoved(Entity entity, int index, IComponent component) {
            if (_collectionsForIndex[index] != null) {
                var collections = _collectionsForIndex[index];
                for (int i = 0, collectionsCount = collections.Count; i < collectionsCount; i++)
                    collections[i].RemoveEntity(entity);
            }
        }

        void onComponentReplaced(Entity entity, int index, IComponent component) {
            if (_collectionsForIndex[index] != null) {
                var collections = _collectionsForIndex[index];
                for (int i = 0, collectionsCount = collections.Count; i < collectionsCount; i++)
                    collections[i].ReplaceEntity(entity);
            }
        }

        void removeFromAllCollections(Entity entity) {
            for (int i = 0, _collectionListCount = _collectionList.Count; i < _collectionListCount; i++)
                _collectionList[i].RemoveEntity(entity);
        }
    }
}

