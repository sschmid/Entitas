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

        public EntityCollection(IEntityMatcher matcher) {
            _matcher = matcher;
        }

        public void AddEntityIfMatching(Entity entity) {
            if (_matcher.Matches(entity)) {
                var added = _entities.Add(entity);
                if (added && OnEntityAdded != null)
                    OnEntityAdded(this, entity);
            }
        }

        public void RemoveEntity(Entity entity) {
            var removed = _entities.Remove(entity);
            if (removed && OnEntityRemoved != null)
                OnEntityRemoved(this, entity);
        }

        public Entity[] GetEntities() {
            return _entities.ToArray();
        }

        public Entity GetSingleEntity() {
            var count = _entities.Count;
            if (count == 0)
                return null;

            if (count > 1)
                throw new SingleEntityException(_matcher);

            return _entities.First();
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

