﻿using System;
using ToolKit;

namespace Entitas {
    public class EntityCollection {
		public event EntityCollectionChange OnEntityAdded;
		public event EntityCollectionChange OnEntityRemoved;
        public event EntityCollectionChange OnEntityWillBeRemoved;

        public delegate void EntityCollectionChange(EntityCollection collection, Entity entity);

        public int Count { get { return _entities.Count; } }

        readonly IEntityMatcher _matcher;
        readonly LinkedListSet<Entity> _entities = new LinkedListSet<Entity>();
        Entity[] _entitiesCache;
        Entity _singleEntityCache;

        public EntityCollection(IEntityMatcher matcher) {
            _matcher = matcher;
        }

        public void AddEntityIfMatching(Entity entity) {
            addEntityIfMatching(entity);
        }

        void addEntityIfMatching(Entity entity) {
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

        public void RemoveEntity(Entity entity) {
            var removed = _entities.Remove(entity);
            if (removed) {
                _entitiesCache = null;
                _singleEntityCache = null;
                if (OnEntityRemoved != null)
                    OnEntityRemoved(this, entity);
            }
        }

		public void EntityWillBeRemoved(Entity entity)
		{
			if(_entities.Contains(entity) && OnEntityWillBeRemoved != null){
				OnEntityWillBeRemoved(this, entity);
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
    }
}