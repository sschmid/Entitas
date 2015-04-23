using System;
using System.Collections.Generic;

namespace Entitas {
    public class Group {
        public event GroupChanged OnEntityAdded;
        public event GroupChanged OnEntityWillBeRemoved;
        public event GroupChanged OnEntityRemoved;

        public delegate void GroupChanged(Group group, Entity entity);

        public int Count { get { return _entities.Count; } }
        public IMatcher matcher { get { return _matcher; } }

        readonly IMatcher _matcher;
        readonly HashSet<Entity> _entities = new HashSet<Entity>(EntityEqualityComparer.comparer);
        Entity[] _entitiesCache;
        Entity _singleEntityCache;

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

        public void UpdateEntity(Entity entity) {
            if (_entities.Contains(entity)) {
                if (OnEntityRemoved != null) {
                    OnEntityRemoved(this, entity);
                }
                if (OnEntityAdded != null) {
                    OnEntityAdded(this, entity);
                }
            }
        }

        public void WillRemoveEntity(Entity entity) {
            if (_entities.Contains(entity) && OnEntityWillBeRemoved != null) {
                OnEntityWillBeRemoved(this, entity);
            }
        }

        void addEntity(Entity entity) {
            var added = _entities.Add(entity);
            if (added) {
                _entitiesCache = null;
                _singleEntityCache = null;
                if (OnEntityAdded != null) {
                    OnEntityAdded(this, entity);
                }
            }
        }

        void removeEntity(Entity entity) {
            var removed = _entities.Remove(entity);
            if (removed) {
                _entitiesCache = null;
                _singleEntityCache = null;
                if (OnEntityRemoved != null) {
                    OnEntityRemoved(this, entity);
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
            return string.Format("Group(" + _matcher + ")");
        }
    }

    public class SingleEntityException : Exception {
        public SingleEntityException(IMatcher matcher) :
            base("Multiple entites exist matching " + matcher) {
        }
    }
}