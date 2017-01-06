using System;
using System.Collections.Generic;

namespace Entitas {

    public partial class Context {

        public event PoolChanged OnEntityCreated;
        public event PoolChanged OnEntityWillBeDestroyed;
        public event PoolChanged OnEntityDestroyed;
        public event GroupChanged OnGroupCreated;
        public event GroupChanged OnGroupCleared;

        public delegate void PoolChanged(Context pool, Entity entity);
        public delegate void GroupChanged(Context pool, Group group);

        public int totalComponents { get { return _totalComponents; } }

        public Stack<IComponent>[] componentPools {
            get { return _componentPools; }
        }

        public PoolMetaData metaData { get { return _metaData; } }

        public int count { get { return _entities.Count; } }

        public int reusableEntitiesCount {
            get { return _reusableEntities.Count; }
        }

        public int retainedEntitiesCount {
            get { return _retainedEntities.Count; }
        }

        readonly int _totalComponents;
        int _creationIndex;

        readonly HashSet<Entity> _entities = new HashSet<Entity>(
            EntityEqualityComparer.comparer
        );

        readonly Stack<Entity> _reusableEntities = new Stack<Entity>();
        readonly HashSet<Entity> _retainedEntities = new HashSet<Entity>(
            EntityEqualityComparer.comparer
        );

        Entity[] _entitiesCache;

        readonly PoolMetaData _metaData;

        readonly Dictionary<IMatcher, Group> _groups =
            new Dictionary<IMatcher, Group>();

        readonly List<Group>[] _groupsForIndex;

        readonly Stack<IComponent>[] _componentPools;
        readonly Dictionary<string, IEntityIndex> _entityIndices;

        // Cache delegates to avoid gc allocations
        Entity.EntityChanged _cachedEntityChanged;
        Entity.ComponentReplaced _cachedComponentReplaced;
        Entity.EntityReleased _cachedEntityReleased;

        public Context(int totalComponents) : this(totalComponents, 0, null) {
        }

        public Context(int totalComponents,
                    int startCreationIndex,
                    PoolMetaData metaData) {
            _totalComponents = totalComponents;
            _creationIndex = startCreationIndex;

            if(metaData != null) {
                _metaData = metaData;

                if(metaData.componentNames.Length != totalComponents) {
                    throw new PoolMetaDataException(this, metaData);
                }
            } else {

                // If Pools.CreatePool() was used to create the pool,
                // we will never end up here.
                // This is a fallback when the pool is created manually.

                var componentNames = new string[totalComponents];
                const string prefix = "Index ";
                for (int i = 0; i < componentNames.Length; i++) {
                    componentNames[i] = prefix + i;
                }
                _metaData = new PoolMetaData(
                    "Unnamed Pool", componentNames, null
                );
            }

            _groupsForIndex = new List<Group>[totalComponents];
            _componentPools = new Stack<IComponent>[totalComponents];
            _entityIndices = new Dictionary<string, IEntityIndex>();

            // Cache delegates to avoid gc allocations
            _cachedEntityChanged = updateGroupsComponentAddedOrRemoved;
            _cachedComponentReplaced = updateGroupsComponentReplaced;
            _cachedEntityReleased = onEntityReleased;
        }

        public virtual Entity CreateEntity() {
            var entity = _reusableEntities.Count > 0
                    ? _reusableEntities.Pop()
                    : new Entity( _totalComponents, _componentPools, _metaData);
            entity._isEnabled = true;
            entity._creationIndex = _creationIndex++;
            entity.Retain(this);
            _entities.Add(entity);
            _entitiesCache = null;
            entity.OnComponentAdded +=_cachedEntityChanged;
            entity.OnComponentRemoved += _cachedEntityChanged;
            entity.OnComponentReplaced += _cachedComponentReplaced;
            entity.OnEntityReleased += _cachedEntityReleased;

            if(OnEntityCreated != null) {
                OnEntityCreated(this, entity);
            }

            return entity;
        }

        public virtual void DestroyEntity(Entity entity) {
            var removed = _entities.Remove(entity);
            if(!removed) {
                throw new PoolDoesNotContainEntityException(
                    "'" + this + "' cannot destroy " + entity + "!",
                    "Did you call pool.DestroyEntity() on a wrong pool?"
                );
            }
            _entitiesCache = null;

            if(OnEntityWillBeDestroyed != null) {
                OnEntityWillBeDestroyed(this, entity);
            }

            entity.destroy();

            if(OnEntityDestroyed != null) {
                OnEntityDestroyed(this, entity);
            }

            if(entity.retainCount == 1) {
                // Can be released immediately without
                // adding to _retainedEntities
                entity.OnEntityReleased -= _cachedEntityReleased;
                _reusableEntities.Push(entity);
                entity.Release(this);
                entity.removeAllOnEntityReleasedHandlers();
            } else {
                _retainedEntities.Add(entity);
                entity.Release(this);
            }
        }

        public virtual void DestroyAllEntities() {
            var entities = GetEntities();
            for (int i = 0; i < entities.Length; i++) {
                DestroyEntity(entities[i]);
            }

            _entities.Clear();

            if(_retainedEntities.Count != 0) {
                throw new PoolStillHasRetainedEntitiesException(this);
            }
        }

        public virtual bool HasEntity(Entity entity) {
            return _entities.Contains(entity);
        }

        public virtual Entity[] GetEntities() {
            if(_entitiesCache == null) {
                _entitiesCache = new Entity[_entities.Count];
                _entities.CopyTo(_entitiesCache);
            }

            return _entitiesCache;
        }

        public virtual Group GetGroup(IMatcher matcher) {
            Group group;
            if(!_groups.TryGetValue(matcher, out group)) {
                group = new Group(matcher);
                var entities = GetEntities();
                for (int i = 0; i < entities.Length; i++) {
                    group.HandleEntitySilently(entities[i]);
                }
                _groups.Add(matcher, group);

                for (int i = 0; i < matcher.indices.Length; i++) {
                    var index = matcher.indices[i];
                    if(_groupsForIndex[index] == null) {
                        _groupsForIndex[index] = new List<Group>();
                    }
                    _groupsForIndex[index].Add(group);
                }

                if(OnGroupCreated != null) {
                    OnGroupCreated(this, group);
                }
            }

            return group;
        }

        public void ClearGroups() {
            foreach(var group in _groups.Values) {
                group.RemoveAllEventHandlers();
                var entities = group.GetEntities();
                for (int i = 0; i < entities.Length; i++) {
                    entities[i].Release(group);
                }

                if(OnGroupCleared != null) {
                    OnGroupCleared(this, group);
                }
            }
            _groups.Clear();

            for (int i = 0; i < _groupsForIndex.Length; i++) {
                _groupsForIndex[i] = null;
            }
        }

        public void AddEntityIndex(string name, IEntityIndex entityIndex) {
            if(_entityIndices.ContainsKey(name)) {
                throw new PoolEntityIndexDoesAlreadyExistException(this, name);
            }

            _entityIndices.Add(name, entityIndex);
        }

        public IEntityIndex GetEntityIndex(string name) {
            IEntityIndex entityIndex;
            if(!_entityIndices.TryGetValue(name, out entityIndex)) {
                throw new PoolEntityIndexDoesNotExistException(this, name);
            }

            return entityIndex;
        }

        public void DeactivateAndRemoveEntityIndices() {
            foreach(var entityIndex in _entityIndices.Values) {
                entityIndex.Deactivate();
            }

            _entityIndices.Clear();
        }

        public void ResetCreationIndex() {
            _creationIndex = 0;
        }

        public void ClearComponentPool(int index) {
            var componentPool = _componentPools[index];
            if(componentPool != null) {
                componentPool.Clear();
            }
        }

        public void ClearComponentPools() {
            for (int i = 0; i < _componentPools.Length; i++) {
                ClearComponentPool(i);
            }
        }

        public void Reset() {
            ClearGroups();
            DestroyAllEntities();
            ResetCreationIndex();

            OnEntityCreated = null;
            OnEntityWillBeDestroyed = null;
            OnEntityDestroyed = null;
            OnGroupCreated = null;
            OnGroupCleared = null;
        }

        public override string ToString() {
            return _metaData.poolName;
        }

        void updateGroupsComponentAddedOrRemoved(
            Entity entity, int index, IComponent component) {
            var groups = _groupsForIndex[index];
            if(groups != null) {
                var events = EntitasCache.GetGroupChangedList();

                    for(int i = 0; i < groups.Count; i++) {
                        events.Add(groups[i].handleEntity(entity));
                    }

                    for(int i = 0; i < events.Count; i++) {
                        var groupChangedEvent = events[i];
                        if(groupChangedEvent != null) {
                            groupChangedEvent(
                                groups[i], entity, index, component
                            );
                        }
                    }

                EntitasCache.PushGroupChangedList(events);
            }
        }

        void updateGroupsComponentReplaced(Entity entity,
                                           int index,
                                           IComponent previousComponent,
                                           IComponent newComponent) {
            var groups = _groupsForIndex[index];
            if(groups != null) {
                for (int i = 0; i < groups.Count; i++) {
                    groups[i].UpdateEntity(
                        entity, index, previousComponent, newComponent
                    );
                }
            }
        }

        void onEntityReleased(Entity entity) {
            if(entity._isEnabled) {
                throw new EntityIsNotDestroyedException(
                    "Cannot release " + entity + "!"
                );
            }
            entity.removeAllOnEntityReleasedHandlers();
            _retainedEntities.Remove(entity);
            _reusableEntities.Push(entity);
        }
    }

    public class PoolDoesNotContainEntityException : EntitasException {
        public PoolDoesNotContainEntityException(string message, string hint) :
            base(message + "\nPool does not contain entity!", hint) {
        }
    }

    public class EntityIsNotDestroyedException : EntitasException {
        public EntityIsNotDestroyedException(string message) :
            base(message + "\nEntity is not destroyed yet!",
                "Did you manually call entity.Release(pool) yourself? " +
                "If so, please don't :)") {
        }
    }

    public class PoolStillHasRetainedEntitiesException : EntitasException {
        public PoolStillHasRetainedEntitiesException(Context pool) : base(
            "'" + pool + "' detected retained entities " +
            "although all entities got destroyed!",
            "Did you release all entities? Try calling pool.ClearGroups() " +
            "and systems.ClearReactiveSystems() before calling " +
            "pool.DestroyAllEntities() to avoid memory leaks.") {
        }
    }

    public class PoolMetaDataException : EntitasException {
        public PoolMetaDataException(Context pool, PoolMetaData poolMetaData) :
            base("Invalid PoolMetaData for '" + pool + "'!\nExpected " +
                 pool.totalComponents + " componentName(s) but got " +
                 poolMetaData.componentNames.Length + ":",
                 string.Join("\n", poolMetaData.componentNames)) {
        }
    }

    public class PoolEntityIndexDoesNotExistException : EntitasException {
        public PoolEntityIndexDoesNotExistException(Context pool, string name) :
            base("Cannot get EntityIndex '" + name + "' from pool '" +
                 pool + "'!", "No EntityIndex with this name has been added.") {
        }
    }

    public class PoolEntityIndexDoesAlreadyExistException : EntitasException {
        public PoolEntityIndexDoesAlreadyExistException(Context pool, string name) :
            base("Cannot add EntityIndex '" + name + "' to pool '" + pool + "'!",
                 "An EntityIndex with this name has already been added.") {
        }
    }

    public class PoolMetaData {

        public readonly string poolName;
        public readonly string[] componentNames;
        public readonly Type[] componentTypes;

        public PoolMetaData(string poolName,
                            string[] componentNames,
                            Type[] componentTypes) {
            this.poolName = poolName;
            this.componentNames = componentNames;
            this.componentTypes = componentTypes;
        }
    }
}
