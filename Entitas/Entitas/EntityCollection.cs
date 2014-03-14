using System;
using System.Collections.Generic;
using ToolKit;

namespace Entitas {
    public class EntityCollection {
        public event EntityCollectionChange OnEntityAdded;
        public event EntityCollectionChange OnEntityRemoved;

        public delegate void EntityCollectionChange(EntityCollection collection, Entity entity);

        readonly IEntityMatcher _matcher;
        readonly OrderedSet<Entity> _entities = new OrderedSet<Entity>();
        Entity[] _entitiesCache;
        Entity _singleEntityCache;

        public EntityCollection(IEntityMatcher matcher) {
            _matcher = matcher;
        }

        public void AddEntityIfMatching(Entity entity) {
            if (_matcher.Matches(entity)) {
                var added = _entities.Add(entity);
                if (added) {
                    _entitiesCache = null;
                    _singleEntityCache = null;
                    if (OnEntityAdded != null)
                        OnEntityAdded(this, entity);
                }
            }
        }

        public void RemoveEntityIfNotMatching(Entity entity) {
            if (!_matcher.Matches(entity))
                RemoveEntity(entity);
        }

        public void RemoveEntity(Entity entity) {
            var removed = _entities.Remove(entity);
            if (removed) {
                _entitiesCache = null;
                _singleEntityCache = null;
                if (OnEntityRemoved != null)
                    OnEntityRemoved(this, entity);
            }
        }

        public Entity[] GetEntities() {
            if (_entitiesCache == null)
                _entitiesCache = _entities.ToArray();

            return _entitiesCache;
        }

        public Entity GetSingleEntity() {
            if (_singleEntityCache == null) {
                var count = _entities.Count;
                if (count == 0)
                    return null;

                if (count > 1)
                    throw new SingleEntityException(_matcher);

                _singleEntityCache = _entities.First();
            }

            return _singleEntityCache;
        }
    }

    public class SingleEntityException : Exception {
        public SingleEntityException(IEntityMatcher matcher) :
            base("Multiple entites exist matching " + matcher) {
        }

        static string getTypesString(IEnumerable<Type> types) {
            const string seperator = ", ";
            var str = string.Empty;
            foreach (var type in types)
                str += type + ", ";

            if (str != string.Empty)
                str = str.Substring(0, str.Length - seperator.Length);

            return str;
        }
    }
}

