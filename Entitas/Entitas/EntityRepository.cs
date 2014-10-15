using ToolKit;
using System.Collections.Generic;
using System;

namespace Entitas {
    public partial class EntityRepository {
        readonly LinkedListSet<Entity> _entities = new LinkedListSet<Entity>();
        readonly Dictionary<IEntityMatcher, EntityCollection> _collections = new Dictionary<IEntityMatcher, EntityCollection>();
        readonly List<EntityCollection>[] _collectionsForIndex;
        readonly ObjectPool<Entity> _entityPool;
        readonly int _totalComponents;
        Entity[] _entitiesCache;

        public EntityRepository(int totalComponents) {
            _totalComponents = totalComponents;
            _collectionsForIndex = new List<EntityCollection>[totalComponents];
            _entityPool = new ObjectPool<Entity>(() => new Entity(_totalComponents));
        }

        public Entity CreateEntity() {
            var entity = _entityPool.Get();
            _entities.Add(entity);
            _entitiesCache = null;
            entity.OnComponentAdded += onComponentAdded;
            entity.OnComponentWillBeRemoved += onComponentWillBeRemoved;
            entity.OnComponentRemoved += onComponentRemoved;
            return entity;
        }

        public void DestroyEntity(Entity entity) {
            entity.RemoveAllComponents();
            entity.OnComponentAdded -= onComponentAdded;
            entity.OnComponentWillBeRemoved -= onComponentWillBeRemoved;
            entity.OnComponentRemoved -= onComponentRemoved;
            _entities.Remove(entity);
            _entitiesCache = null;
            _entityPool.Push(entity);
        }

        public void DestroyAllEntities() {
            var entities = GetEntities();
            for (int i = 0, entitiesLength = entities.Length; i < entitiesLength; i++) {
                DestroyEntity(entities[i]);
            }
        }

        public bool HasEntity(Entity entity) {
            return _entities.Contains(entity);
        }

        public Entity[] GetEntities() {
            if (_entitiesCache == null) {
                _entitiesCache = _entities.ToArray();
            }

            return _entitiesCache;
        }

        public EntityCollection GetCollection(IEntityMatcher matcher) {
            if (!_collections.ContainsKey(matcher)) {
                var collection = new EntityCollection(matcher);
                var entities = GetEntities();
                for (int i = 0, entitiesLength = entities.Length; i < entitiesLength; i++) {
                    collection.AddEntityIfMatching(entities[i]);
                }
                _collections.Add(matcher, collection);

                for (int i = 0, indicesLength = matcher.indices.Length; i < indicesLength; i++) {
                    var index = matcher.indices[i];
                    if (_collectionsForIndex[index] == null) {
                        _collectionsForIndex[index] = new List<EntityCollection>();
                    }
                    _collectionsForIndex[index].Add(collection);
                }
            }

            return _collections[matcher];
        }

        void onComponentAdded(Entity entity, int index, IComponent component) {
            var collections = _collectionsForIndex[index];
            if (collections != null) {
                for (int i = 0, collectionsCount = collections.Count; i < collectionsCount; i++) {
                    collections[i].AddEntityIfMatching(entity);
                }
            }
        }

        void onComponentWillBeRemoved(Entity entity, int index, IComponent component) {
            var collections = _collectionsForIndex[index];
            if (collections != null) {
                for (int i = 0, collectionsCount = collections.Count; i < collectionsCount; i++) {
                    collections[i].WillRemoveEntity(entity);
                }
            }
        }

        void onComponentRemoved(Entity entity, int index, IComponent component) {
            var collections = _collectionsForIndex[index];
            if (collections != null) {
                for (int i = 0, collectionsCount = collections.Count; i < collectionsCount; i++) {
                    collections[i].RemoveEntity(entity);
                }
            }
        }
    }
}

