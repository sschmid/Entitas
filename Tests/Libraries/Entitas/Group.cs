using System;
using System.Collections.Generic;
using System.Linq;

namespace Entitas {
    public class Group {
        public event GroupChanged OnEntityAdded;
        public event GroupChanged OnEntityRemoved;
        public event GroupUpdated OnEntityUpdated;

        public delegate void GroupChanged(Group group, Entity entity, int index, IComponent component);
        public delegate void GroupUpdated(Group group, Entity entity, int index, IComponent previousComponent, IComponent newComponent);

        public int count { get { return _entities.Count; } }
        public IMatcher matcher { get { return _matcher; } }

        readonly IMatcher _matcher;

        readonly HashSet<Entity> _entities = new HashSet<Entity>(EntityEqualityComparer.comparer);
        Entity[] _entitiesCache;
        Entity _singleEntityCache;
        string _toStringCache;

        public Group(IMatcher matcher) {
            _matcher = matcher;
        }

        public void HandleEntitySilently(Entity entity) {
            if (_matcher.Matches(entity)) {
                addEntitySilently(entity);
            } else {
                removeEntitySilently(entity);
            }
        }

        public void HandleEntity(Entity entity, int index, IComponent component) {
            if (_matcher.Matches(entity)) {
                addEntity(entity, index, component);
            } else {
                removeEntity(entity, index, component);
            }
        }

        internal GroupChanged handleEntity(Entity entity) {
            return _matcher.Matches(entity)
                        ? addEntity(entity)
                        : removeEntity(entity);
        }

        public void UpdateEntity(Entity entity, int index, IComponent previousComponent, IComponent newComponent) {
            if (_entities.Contains(entity)) {
                if (OnEntityRemoved != null) {
                    OnEntityRemoved(this, entity, index, previousComponent);
                }
                if (OnEntityAdded != null) {
                    OnEntityAdded(this, entity, index, newComponent);
                }
                if (OnEntityUpdated != null) {
                    OnEntityUpdated(this, entity, index, previousComponent, newComponent);
                }
            }
        }

        public void RemoveAllEventHandlers() {
            OnEntityAdded = null;
            OnEntityRemoved = null;
            OnEntityUpdated = null;
        }

        bool addEntitySilently(Entity entity) {
            var added = _entities.Add(entity);
            if (added) {
                _entitiesCache = null;
                _singleEntityCache = null;
                entity.Retain(this);
            }

            return added;
        }

        void addEntity(Entity entity, int index, IComponent component) {
            if (addEntitySilently(entity) && OnEntityAdded != null) {
                OnEntityAdded(this, entity, index, component);
            }
        }

        GroupChanged addEntity(Entity entity) {
            return addEntitySilently(entity) ? OnEntityAdded : null;
        }

        bool removeEntitySilently(Entity entity) {
            var removed = _entities.Remove(entity);
            if (removed) {
                _entitiesCache = null;
                _singleEntityCache = null;
                entity.Release(this);
            }

            return removed;
        }

        void removeEntity(Entity entity, int index, IComponent component) {
            var removed = _entities.Remove(entity);
            if (removed) {
                _entitiesCache = null;
                _singleEntityCache = null;
                if (OnEntityRemoved != null) {
                    OnEntityRemoved(this, entity, index, component);
                }
                entity.Release(this);
            }
        }

        GroupChanged removeEntity(Entity entity) {
            return removeEntitySilently(entity) ? OnEntityRemoved : null;
        }

        public bool ContainsEntity(Entity entity) {
            return _entities.Contains(entity);
        }

        public Entity[] GetEntities() {
            if (_entitiesCache == null) {
                _entitiesCache = new Entity[_entities.Count];
                _entities.CopyTo(_entitiesCache);
            }

            return _entitiesCache;
        }

        public Entity GetSingleEntity() {
            if (_singleEntityCache == null) {
                var c = _entities.Count;
                if (c == 1) {
                    using (var enumerator = _entities.GetEnumerator()) {
                        enumerator.MoveNext();
                        _singleEntityCache = enumerator.Current;
                    }
                } else if (c == 0) {
                    return null;
                } else {
                    throw new GroupSingleEntityException(this);
                }
            }

            return _singleEntityCache;
        }

        public override string ToString() {
            if (_toStringCache == null) {
                _toStringCache = "Group(" + _matcher + ")";
            }
            return _toStringCache;
        }
    }

    public class GroupSingleEntityException : EntitasException {
        public GroupSingleEntityException(Group group) :
            base("Cannot get the single entity from " + group + "!\nGroup contains " + group.count + " entities:",
                string.Join("\n", group.GetEntities().Select(e => e.ToString()).ToArray())) {
        }
    }
}