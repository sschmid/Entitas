using System;
using System.Collections.Generic;

namespace Entitas {
    public partial class Pool {

        public event PoolChanged OnEntityCreated;
        public event PoolChanged OnEntityWillBeDestroyed;
        public event PoolChanged OnEntityDestroyed;
        public event GroupChanged OnGroupCreated;

        public delegate void PoolChanged(Pool pool, Entity entity);
        public delegate void GroupChanged(Pool pool, Group group);

        public int totalComponents { get { return _totalComponents; } }
        public int Count { get { return _entities.Count; } }

        protected readonly HashSet<Entity> _entities = new HashSet<Entity>(EntityEqualityComparer.comparer);
        protected readonly Dictionary<IMatcher, Group> _groups = new Dictionary<IMatcher, Group>();
        protected readonly List<Group>[] _groupsForIndex;
        readonly int _totalComponents;
        int _creationIndex;
        Entity[] _entitiesCache;

        public int pooledEntitiesCount { get { return _entityPool.Count; } }
        readonly Stack<Entity> _entityPool = new Stack<Entity>();

        public Pool(int totalComponents) : this(totalComponents, 0) {
        }

        public Pool(int totalComponents, int startCreationIndex) {
            _totalComponents = totalComponents;
            _creationIndex = startCreationIndex;
            _groupsForIndex = new List<Group>[totalComponents];

            _entityPool = new Stack<Entity>();
        }

        public virtual Entity CreateEntity() {
            var entity = _entityPool.Count > 0 ? _entityPool.Pop() : new Entity(_totalComponents);

            entity._isEnabled = true;
            entity._creationIndex = _creationIndex++;
            entity.ResetRefCount();
            entity.Retain();
            _entities.Add(entity);
            _entitiesCache = null;
            entity.OnComponentAdded += updateGroupsComponentAddedOrRemoved;
            entity.OnComponentReplaced += updateGroupsComponentReplaced;
            entity.OnComponentRemoved += updateGroupsComponentAddedOrRemoved;
            entity.OnEntityReleased += reuseAfterEntityReleased;

            if (OnEntityCreated != null) {
                OnEntityCreated(this, entity);
            }

            return entity;
        }

        public virtual void DestroyEntity(Entity entity) {
            var removed = _entities.Remove(entity);
            if (!removed) {
                throw new PoolDoesNotContainEntityException(entity,
                    "Could not destroy entity!");
            }
            _entitiesCache = null;
            
            if (OnEntityWillBeDestroyed != null) {
                OnEntityWillBeDestroyed(this, entity);
            }

            entity.RemoveAllComponents();
            entity.OnComponentAdded -= updateGroupsComponentAddedOrRemoved;
            entity.OnComponentReplaced -= updateGroupsComponentReplaced;
            entity.OnComponentRemoved -= updateGroupsComponentAddedOrRemoved;
            entity._isEnabled = false;

            _nonReusableEntities.Add(entity);
            entity.Release();

            if (OnEntityDestroyed != null) {
                OnEntityDestroyed(this, entity);
            }
        }

        public virtual void DestroyAllEntities() {
            var entities = GetEntities();
            for (int i = 0, entitiesLength = entities.Length; i < entitiesLength; i++) {
                DestroyEntity(entities[i]);
            }
        }

        public virtual bool HasEntity(Entity entity) {
            return _entities.Contains(entity);
        }

        public virtual Entity[] GetEntities() {
            if (_entitiesCache == null) {
                _entitiesCache = new Entity[_entities.Count];
                _entities.CopyTo(_entitiesCache);
            }

            return _entitiesCache;
        }

        public virtual Group GetGroup(IMatcher matcher) {
            Group group;
            if (!_groups.TryGetValue(matcher, out group)) {
                group = new Group(matcher);
                var entities = GetEntities();
                for (int i = 0, entitiesLength = entities.Length; i < entitiesLength; i++) {
                    group.HandleEntity(entities[i]);
                }
                _groups.Add(matcher, group);

                for (int i = 0, indicesLength = matcher.indices.Length; i < indicesLength; i++) {
                    var index = matcher.indices[i];
                    if (_groupsForIndex[index] == null) {
                        _groupsForIndex[index] = new List<Group>();
                    }
                    _groupsForIndex[index].Add(group);
                }

                if (OnGroupCreated != null) {
                    OnGroupCreated(this, group);
                }
            }

            return group;
        }

        protected void updateGroupsComponentAddedOrRemoved(Entity entity, int index, IComponent component) {
            var groups = _groupsForIndex[index];
            if (groups != null) {
                for (int i = 0, groupsCount = groups.Count; i < groupsCount; i++) {
                    groups[i].HandleEntity(entity, index, component);
                }
            }
        }

        protected void updateGroupsComponentReplaced(Entity entity, int index, IComponent previousComponent, IComponent newComponent) {
            var groups = _groupsForIndex[index];
            if (groups != null) {
                for (int i = 0, groupsCount = groups.Count; i < groupsCount; i++) {
                    groups[i].UpdateEntity(entity, index, previousComponent, newComponent);
                }
            }
        }
    }

    public class PoolDoesNotContainEntityException : Exception {
        public PoolDoesNotContainEntityException(Entity entity, string message) :
            base(message + "\nPool does not contain entity " + entity) {
        }
    }

    public partial class Pool {
        HashSet<Entity> _nonReusableEntities = new HashSet<Entity>();

        protected void reuseAfterEntityReleased(Entity entity) {
            entity.OnEntityReleased -= reuseAfterEntityReleased;
            _entityPool.Push(entity);
            _nonReusableEntities.Remove(entity);
        }
    }
}

