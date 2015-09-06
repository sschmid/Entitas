using System;
using System.Collections.Generic;

namespace Entitas {

    /// <summary>
    /// Represents a Group of Entities defined by a <see cref="IMatcher"/>.
    /// Responsible for maintaining a list of entities conforming to that <see cref="IMatcher"/> and the events surrounding it as defined by <see cref="OnEntityAdded"/>, <see cref="OnEntityRemoved"/> and <see cref="OnEntityUpdated"/>.
    /// </summary>
    public class Group {

        /// <summary>
        /// Called whenever an Entity is removed from the group, or a Component to which this Group is subscribed on an Entity is changed. <para/>
        /// Upon change, the event is called with the old Component, and called before <see cref="OnEntityAdded"/> and <see cref="OnEntityUpdated"/>.
        /// </summary>
        public event GroupChanged OnEntityRemoved;

        /// <summary>
        /// Called whenever an Entity is added to the group, or a Component to which this Group is subscribed on an Entity is changed. <para/>
        /// Upon change, the event is called with the new Component, and called before <see cref="OnEntityUpdated"/>, but after <see cref="OnEntityRemoved"/>.
        /// </summary>
        public event GroupChanged OnEntityAdded;

        /// <summary>
        /// Called whenever an Entity in this group is updated (i.e.; has its Component replaced).
        /// This event is called after <see cref="OnEntityRemoved"/> and <see cref="OnEntityAdded"/>
        /// </summary>
        public event GroupUpdated OnEntityUpdated;

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

        public void HandleEntitySilently(Entity entity) {
            if (_matcher.Matches(entity)) {
                addEntitySilently(entity);
            } else {
                removeEntitySilently(entity);
            }
        }

        /// <summary>
        /// Called whenever a Component is removed or added to an Entity and will add or remove the Entity from the Group. 
        /// </summary>
        public void HandleEntity(Entity entity, int index, IComponent component) {
            if (_matcher.Matches(entity)) {
                addEntity(entity, index, component);
            } else {
                removeEntity(entity, index, component);
            }
        }

        /// <summary>
        /// Called whenever
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="index"></param>
        /// <param name="previousComponent"></param>
        /// <param name="newComponent"></param>
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

        void addEntitySilently(Entity entity) {
            var added = _entities.Add(entity);
            if (added) {
                _entitiesCache = null;
                _singleEntityCache = null;
                entity.Retain();
            }
        }

        void addEntity(Entity entity, int index, IComponent component) {
            var added = _entities.Add(entity);
            if (added) {
                _entitiesCache = null;
                _singleEntityCache = null;
                entity.Retain();
                if (OnEntityAdded != null) {
                    OnEntityAdded(this, entity, index, component);
                }
            }
        }

        void removeEntitySilently(Entity entity) {
            var removed = _entities.Remove(entity);
            if (removed) {
                _entitiesCache = null;
                _singleEntityCache = null;
                entity.Release();
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
                entity.Release();
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
                    using (var enumerator = _entities.GetEnumerator()) {
                        enumerator.MoveNext();
                        _singleEntityCache = enumerator.Current;
                    }
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
            base("Multiple entities exist matching " + matcher) {
        }
    }
}