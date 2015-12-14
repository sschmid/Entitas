using System;
using System.Collections.Generic;

namespace Entitas {
    public partial class Pool {

        public event PoolChanged OnEntityCreated;
        public event PoolChanged OnEntityWillBeDestroyed;
        public event PoolChanged OnEntityDestroyed;
        public event GroupChanged OnGroupCreated;
        public event GroupChanged OnGroupCleared;

        public delegate void PoolChanged(Pool pool, Entity entity);
        public delegate void GroupChanged(Pool pool, Group group);

        public int totalComponents { get { return _totalComponents; } }
        public int count { get { return _entities.Count; } }
        public int reusableEntitiesCount { get { return _reusableEntities.Count; } }
        public int retainedEntitiesCount { get { return _retainedEntities.Count; } }

        protected readonly HashSet<Entity> _entities = new HashSet<Entity>(EntityEqualityComparer.comparer);
        protected readonly Dictionary<IMatcher, Group> _groups = new Dictionary<IMatcher, Group>();
        protected readonly List<Group>[] _groupsForIndex;
        readonly Stack<Entity> _reusableEntities = new Stack<Entity>();
        readonly HashSet<Entity> _retainedEntities = new HashSet<Entity>();

        readonly int _totalComponents;
        int _creationIndex;
        Entity[] _entitiesCache;

        // Cache delegates to avoid gc allocations
        Entity.EntityChanged _cachedUpdateGroupsComponentAddedOrRemoved;
        Entity.ComponentReplaced _cachedUpdateGroupsComponentReplaced;
        Entity.EntityReleased _cachedOnEntityReleased;

        public Pool(int totalComponents) : this(totalComponents, 0) {
        }

        public Pool(int totalComponents, int startCreationIndex) {
            _totalComponents = totalComponents;
            _creationIndex = startCreationIndex;
            _groupsForIndex = new List<Group>[totalComponents];

            // Cache delegates to avoid gc allocations
            _cachedUpdateGroupsComponentAddedOrRemoved = updateGroupsComponentAddedOrRemoved;
            _cachedUpdateGroupsComponentReplaced = updateGroupsComponentReplaced;
            _cachedOnEntityReleased = onEntityReleased;
        }

        public virtual Entity CreateEntity() {
            var entity = _reusableEntities.Count > 0 ? _reusableEntities.Pop() : new Entity(_totalComponents);
            entity._isEnabled = true;
            entity._creationIndex = _creationIndex++;
            entity.Retain(this);
            _entities.Add(entity);
            _entitiesCache = null;
            entity.OnComponentAdded += _cachedUpdateGroupsComponentAddedOrRemoved;
            entity.OnComponentRemoved += _cachedUpdateGroupsComponentAddedOrRemoved;
            entity.OnComponentReplaced += _cachedUpdateGroupsComponentReplaced;
            entity.OnEntityReleased += _cachedOnEntityReleased;

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

            entity.destroy();

            if (OnEntityDestroyed != null) {
                OnEntityDestroyed(this, entity);
            }

            if (entity.retainCount == 1) {
                entity.OnEntityReleased -= _cachedOnEntityReleased;
                _reusableEntities.Push(entity);
            } else {
                _retainedEntities.Add(entity);
            }
            entity.Release(this);
        }

        public virtual void DestroyAllEntities() {
            var entities = GetEntities();
            for (int i = 0, entitiesLength = entities.Length; i < entitiesLength; i++) {
                DestroyEntity(entities[i]);
            }

            if (_retainedEntities.Count != 0) {
                throw new PoolStillHasRetainedEntitiesException();
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
                    group.HandleEntitySilently(entities[i]);
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

        public void ClearGroups() {
            foreach (var group in _groups.Values) {
                group.RemoveAllEventHandlers();
                for (int i = 0, n = group.GetEntities().Length; i < n; i++) {
                    group.GetEntities()[i].Release(group);
                }

                if (OnGroupCleared != null) {
                    OnGroupCleared(this, group);
                }
            }
            _groups.Clear();

            for (int i = 0, groupsForIndexLength = _groupsForIndex.Length; i < groupsForIndexLength; i++) {
                _groupsForIndex[i] = null;
            }
        }

        public void ResetCreationIndex() {
            _creationIndex = 0;
        }

        protected void updateGroupsComponentAddedOrRemoved(Entity entity, int index, IComponent component) {
            var groups = _groupsForIndex[index];
            if (groups != null) {
                var events = new List<Group.GroupChanged>(groups.Count);
                for (int i = 0, groupsCount = groups.Count; i < groupsCount; i++) {
                    events.Add(groups[i].handleEntity(entity));
                }
                for (int i = 0, eventsCount = events.Count; i < eventsCount; i++) {
                    var groupChangedEvent = events[i];
                    if (groupChangedEvent != null) {
                        groupChangedEvent(groups[i], entity, index, component);
                    }
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

        protected void onEntityReleased(Entity entity) {
            if(entity._isEnabled){
                throw new EntityIsNotDestroyedException("Cannot release entity.");
            }
            entity.OnEntityReleased -= _cachedOnEntityReleased;
            _retainedEntities.Remove(entity);
            _reusableEntities.Push(entity);
        }
    }

    public class PoolDoesNotContainEntityException : Exception {
        public PoolDoesNotContainEntityException(Entity entity, string message) :
            base(message + "\nPool does not contain entity " + entity) {
        }
    }

    public class EntityIsNotDestroyedException : Exception {
        public EntityIsNotDestroyedException(string message) :
            base(message + "\nEntity is not destroyed yet!") {
        }
    }

    public class PoolStillHasRetainedEntitiesException : Exception {
    }
}

