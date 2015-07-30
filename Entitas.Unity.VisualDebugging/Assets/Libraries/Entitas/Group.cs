using System;
using System.Collections.Generic;

namespace Entitas {
    public class Group {
        public event GroupChanged OnEntityAdded;
        public event GroupUpdated OnEntityUpdated;
        public event GroupChanged OnEntityRemoved;

        public delegate void GroupChanged(Group group, Entity entity, int index, IComponent component);
        public delegate void GroupUpdated(Group group, Entity entity, int index, IComponent previousComponent, IComponent newComponent);

        public int Count { get { return _entities.Count; } }
        public IMatcher matcher { get { return _matcher; } }

        readonly IMatcher _matcher;
        readonly HashSet<Entity> _entities = new HashSet<Entity>(EntityEqualityComparer.comparer);
        Entity[] _entitiesCache;
        Entity _singleEntityCache;
        string _toStringCache;

        public Group(IMatcher matcher) {
            _matcher = matcher;
        }

        public void HandleEntity(Entity entity) {
            if (_matcher.Matches(entity)) {
                addEntity(entity);
            } else {
                removeEntity(entity);
            }
        }

        public void HandleEntity(Entity entity, int index, IComponent component) {
            if (_matcher.Matches(entity)) {
                addEntity(entity, index, component);
            } else {
                removeEntity(entity, index, component);
            }
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

        void addEntity(Entity entity) {
            var added = _entities.Add(entity);
            if (added) {
                _entitiesCache = null;
                _singleEntityCache = null;
            }
        }

        void addEntity(Entity entity, int index, IComponent component) {
            var added = _entities.Add(entity);
            if (added) {
                _entitiesCache = null;
                _singleEntityCache = null;
                if (OnEntityAdded != null) {
                    OnEntityAdded(this, entity, index, component);
                }
            }
        }

        void removeEntity(Entity entity) {
            var removed = _entities.Remove(entity);
            if (removed) {
                _entitiesCache = null;
                _singleEntityCache = null;
            }
        }

        void removeEntity(Entity entity, int index, IComponent component) {
            var removed = _entities.Remove(entity);
            if (removed) {
                _entitiesCache = null;
                _singleEntityCache = null;
                if (OnEntityRemoved != null) {
                    OnEntityRemoved(this, entity, index, component);
                }
            }
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
                var count = _entities.Count;
                if (count == 1) {
                    _singleEntityCache = System.Linq.Enumerable.First(_entities);
                } else {
                    if (count == 0) {
                        return null;
                    }

                    if (count > 1) {
                        throw new SingleEntityException(_matcher);
                    }
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

    public class SingleEntityException : Exception {
        public SingleEntityException(IMatcher matcher) :
            base("Multiple entites exist matching " + matcher) {
        }
    }
}