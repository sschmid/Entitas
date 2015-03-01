using System;
using System.Collections.Generic;

namespace Entitas {
    public partial class Pool {

        public int Count { get { return _entities.Count; } }

        readonly HashSet<Entity> _entities = new HashSet<Entity>(EntityEqualityComparer.comparer);
        readonly Dictionary<IMatcher, Group> _groups = new Dictionary<IMatcher, Group>();
        readonly List<Group>[] _groupsForIndex;
        readonly ObjectPool _entityPool;
        readonly int _totalComponents;
        int _creationIndex;
        Entity[] _entitiesCache;

        public Pool(int totalComponents) : this(totalComponents, 0) {
        }

        public Pool(int totalComponents, int startCreationIndex) {
            _totalComponents = totalComponents;
            _creationIndex = startCreationIndex;
            _groupsForIndex = new List<Group>[totalComponents];
            _entityPool = new ObjectPool(() => new Entity(_totalComponents));
        }

        public Entity CreateEntity() {
            var entity = _entityPool.Get();
            entity._creationIndex = _creationIndex++;
            _entities.Add(entity);
            _entitiesCache = null;
            entity.OnComponentAdded += onComponentAdded;
            entity.OnComponentReplaced += onComponentReplaced;
            entity.OnComponentWillBeRemoved += onComponentWillBeRemoved;
            entity.OnComponentRemoved += onComponentRemoved;
            return entity;
        }

        public void DestroyEntity(Entity entity) {
            entity.RemoveAllComponents();
            entity.OnComponentAdded -= onComponentAdded;
            entity.OnComponentReplaced -= onComponentReplaced;
            entity.OnComponentWillBeRemoved -= onComponentWillBeRemoved;
            entity.OnComponentRemoved -= onComponentRemoved;
            _entities.Remove(entity);
            _entitiesCache = null;
            _entityPool.Push(entity);
        }

        public void DestroyAllEntities() {
            var entities = GetEntities();
            foreach (var entity in entities) {
                DestroyEntity(entity);
            }
        }

        public bool HasEntity(Entity entity) {
            return _entities.Contains(entity);
        }

        public Entity[] GetEntities() {
            if (_entitiesCache == null) {
                _entitiesCache = new Entity[_entities.Count];
                _entities.CopyTo(_entitiesCache);
            }

            return _entitiesCache;
        }

        public Group GetGroup(IMatcher matcher) {
            Group group;
            if (!_groups.TryGetValue(matcher, out group)) {
                group = new Group(matcher);
                foreach (var entity in _entities) {
                    group.AddEntityIfMatching(entity);
                }
                _groups.Add(matcher, group);

                for (int i = 0, indicesLength = matcher.indices.Length; i < indicesLength; i++) {
                    var index = matcher.indices[i];
                    if (_groupsForIndex[index] == null) {
                        _groupsForIndex[index] = new List<Group>();
                    }
                    _groupsForIndex[index].Add(group);
                }
            }

            return group;
        }

        void onComponentAdded(Entity entity, int index, IComponent component) {
            var groups = _groupsForIndex[index];
            if (groups != null) {
                for (int i = 0, groupsCount = groups.Count; i < groupsCount; i++) {
                    groups[i].AddEntityIfMatching(entity);
                }
            }
        }

        void onComponentReplaced(Entity entity, int index, IComponent component) {
            var groups = _groupsForIndex[index];
            if (groups != null) {
                for (int i = 0, groupsCount = groups.Count; i < groupsCount; i++) {
                    groups[i].UpdateEntity(entity);
                }
            }
        }

        void onComponentWillBeRemoved(Entity entity, int index, IComponent component) {
            var groups = _groupsForIndex[index];
            if (groups != null) {
                for (int i = 0, groupsCount = groups.Count; i < groupsCount; i++) {
                    groups[i].WillRemoveEntity(entity);
                }
            }
        }

        void onComponentRemoved(Entity entity, int index, IComponent component) {
            var groups = _groupsForIndex[index];
            if (groups != null) {
                for (int i = 0, groupsCount = groups.Count; i < groupsCount; i++) {
                    groups[i].RemoveEntity(entity);
                }
            }
        }
    }

    class ObjectPool {
        public int Count { get { return _pool.Count; } }

        readonly Func<Entity> _factoryMethod;
        readonly Stack<Entity> _pool = new Stack<Entity>();

        public ObjectPool(Func<Entity> factoryMethod) {
            _factoryMethod = factoryMethod;
        }

        public Entity Get() {
            return _pool.Count > 0 ? _pool.Pop() : _factoryMethod();
        }

        public void Push(Entity entity) {
            _pool.Push(entity);
        }
    }
}

