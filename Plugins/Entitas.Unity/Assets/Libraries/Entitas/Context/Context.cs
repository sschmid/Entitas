using System.Collections.Generic;
using Entitas.Api;

namespace Entitas {

    public class Context<TEntity> : IContext<TEntity> where TEntity : class, IEntity, new() {

        public event ContextEntityChanged OnEntityCreated;
        public event ContextEntityChanged OnEntityWillBeDestroyed;
        public event ContextEntityChanged OnEntityDestroyed;
        public event ContextGroupChanged OnGroupCreated;
        public event ContextGroupChanged OnGroupCleared;

        public int totalComponents { get { return _totalComponents; } }

        public Stack<IComponent>[] componentPools { get { return _componentPools; } }
        public ContextInfo contextInfo { get { return _contextInfo; } }
        
        public int count { get { return _entities.Count; } }
        public int reusableEntitiesCount { get { return _reusableEntities.Count; } }
        public int retainedEntitiesCount { get { return _retainedEntities.Count; } }

        readonly int _totalComponents;

        readonly Stack<IComponent>[] _componentPools;
        readonly ContextInfo _contextInfo;

        readonly HashSet<TEntity> _entities = new HashSet<TEntity>(EntityEqualityComparer<TEntity>.comparer);
        readonly Stack<TEntity> _reusableEntities = new Stack<TEntity>();
        readonly HashSet<TEntity> _retainedEntities = new HashSet<TEntity>(EntityEqualityComparer<TEntity>.comparer);

        readonly Dictionary<IMatcher<TEntity>, IGroup<TEntity>> _groups = new Dictionary<IMatcher<TEntity>, IGroup<TEntity>>();
        readonly List<IGroup<TEntity>>[] _groupsForIndex;

        readonly Dictionary<string, IEntityIndex> _entityIndices;

        int _creationIndex;

        TEntity[] _entitiesCache;

        // Cache delegates to avoid gc allocations
        EntityComponentChanged _cachedEntityChanged;
        EntityComponentReplaced _cachedComponentReplaced;
        EntityReleased _cachedEntityReleased;

        public Context(int totalComponents) : this(totalComponents, 0, null) {
        }

        public Context(int totalComponents, int startCreationIndex, ContextInfo contextInfo) {
            _totalComponents = totalComponents;
            _creationIndex = startCreationIndex;

            if(contextInfo != null) {
                _contextInfo = contextInfo;
                if(contextInfo.componentNames.Length != totalComponents) {
                    throw new ContextInfoException(this, contextInfo);
                }
            } else {
                _contextInfo = createDefaultContextInfo();
            }

            _groupsForIndex = new List<IGroup<TEntity>>[totalComponents];
            _componentPools = new Stack<IComponent>[totalComponents];
            _entityIndices = new Dictionary<string, IEntityIndex>();

            // Cache delegates to avoid gc allocations
            _cachedEntityChanged = updateGroupsComponentAddedOrRemoved;
            _cachedComponentReplaced = updateGroupsComponentReplaced;
            _cachedEntityReleased = onEntityReleased;
        }

        ContextInfo createDefaultContextInfo() {
            var componentNames = new string[_totalComponents];
            const string prefix = "Index ";
            for(int i = 0; i < componentNames.Length; i++) {
                componentNames[i] = prefix + i;
            }

            return new ContextInfo("Unnamed Context", componentNames, null);
        }

        public virtual TEntity CreateEntity() {
            TEntity entity;

            // TODO UNIT TEST
            // Test Reactivate
            // Test Initialize
            if(_reusableEntities.Count > 0) {
                entity = _reusableEntities.Pop();
                // TODO
                entity.Reactivate(_creationIndex++);
            } else {
                entity = new TEntity();
                // TODO
                entity.Initialize(_creationIndex++, _totalComponents, _componentPools, _contextInfo);
            }

            _entities.Add(entity);
            entity.Retain(this);
            _entitiesCache = null;
            entity.OnComponentAdded += _cachedEntityChanged;
            entity.OnComponentRemoved += _cachedEntityChanged;
            entity.OnComponentReplaced += _cachedComponentReplaced;
            entity.OnEntityReleased += _cachedEntityReleased;

            if(OnEntityCreated != null) {
                OnEntityCreated(this, entity);
            }

            return entity;
        }

        public virtual void DestroyEntity(TEntity entity) {
            var removed = _entities.Remove(entity);
            if(!removed) {
                throw new ContextDoesNotContainEntityException(
                    "'" + this + "' cannot destroy " + entity + "!",
                    "Did you call context.DestroyEntity() on a wrong context?"
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
            for(int i = 0; i < entities.Length; i++) {
                DestroyEntity(entities[i]);
            }

            _entities.Clear();

            if(_retainedEntities.Count != 0) {
                throw new ContextStillHasRetainedEntitiesException(this);
            }
        }

        public virtual bool HasEntity(TEntity entity) {
            return _entities.Contains(entity);
        }

        public virtual TEntity[] GetEntities() {
            if(_entitiesCache == null) {
                _entitiesCache = new TEntity[_entities.Count];
                _entities.CopyTo(_entitiesCache);
            }

            return _entitiesCache;
        }

        public virtual IGroup<TEntity> GetGroup(IMatcher<TEntity> matcher) {
            IGroup<TEntity> group;
            if(!_groups.TryGetValue(matcher, out group)) {
                group = new Group<TEntity>(matcher);
                var entities = GetEntities();
                for(int i = 0; i < entities.Length; i++) {
                    group.HandleEntitySilently(entities[i]);
                }
                _groups.Add(matcher, group);

                for(int i = 0; i < matcher.indices.Length; i++) {
                    var index = matcher.indices[i];
                    if(_groupsForIndex[index] == null) {
                        _groupsForIndex[index] = new List<IGroup<TEntity>>();
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
                for(int i = 0; i < entities.Length; i++) {
                    entities[i].Release(group);
                }

                if(OnGroupCleared != null) {
                    OnGroupCleared(this, group);
                }
            }
            _groups.Clear();

            for(int i = 0; i < _groupsForIndex.Length; i++) {
                _groupsForIndex[i] = null;
            }
        }

        public void AddEntityIndex(string name, IEntityIndex entityIndex) {
            if(_entityIndices.ContainsKey(name)) {
                throw new ContextEntityIndexDoesAlreadyExistException(this, name);
            }

            _entityIndices.Add(name, entityIndex);
        }

        public IEntityIndex GetEntityIndex(string name) {
            IEntityIndex entityIndex;
            if(!_entityIndices.TryGetValue(name, out entityIndex)) {
                throw new ContextEntityIndexDoesNotExistException(this, name);
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
            for(int i = 0; i < _componentPools.Length; i++) {
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
            return _contextInfo.name;
        }

        void updateGroupsComponentAddedOrRemoved(IEntity entity, int index, IComponent component) {
            var groups = _groupsForIndex[index];
            if(groups != null) {
                var events = EntitasCache.GetGroupChangedList<TEntity>();

                    var tEntity = (TEntity)entity;

                    for(int i = 0; i < groups.Count; i++) {
                        events.Add(groups[i].handleEntity(tEntity));
                    }

                    for(int i = 0; i < events.Count; i++) {
                        var groupChangedEvent = events[i];
                        if(groupChangedEvent != null) {
                            groupChangedEvent(
                                groups[i], tEntity, index, component
                            );
                        }
                    }

                EntitasCache.PushGroupChangedList<TEntity>(events);
            }
        }

        void updateGroupsComponentReplaced(IEntity entity, int index, IComponent previousComponent, IComponent newComponent) {
            var groups = _groupsForIndex[index];
            if(groups != null) {

                var tEntity = (TEntity)entity;

                for(int i = 0; i < groups.Count; i++) {
                    groups[i].UpdateEntity(
                        tEntity, index, previousComponent, newComponent
                    );
                }
            }
        }

        void onEntityReleased(IEntity entity) {
            if(entity.isEnabled) {
                throw new EntityIsNotDestroyedException(
                    "Cannot release " + entity + "!"
                );
            }
            var tEntity = (TEntity)entity;
            entity.removeAllOnEntityReleasedHandlers();
            _retainedEntities.Remove(tEntity);
            _reusableEntities.Push(tEntity);
        }
    }
}
