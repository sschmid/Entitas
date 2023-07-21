using System.Collections.Generic;
using System.Linq;

namespace Entitas
{
    /// Use context.GetGroup(matcher) to get a group of entities which match
    /// the specified matcher. Calling context.GetGroup(matcher) with the
    /// same matcher will always return the same instance of the group.
    /// The created group is managed by the context and will always be up to date.
    /// It will automatically add entities that match the matcher or
    /// remove entities as soon as they don't match the matcher anymore.
    public class Group<TEntity> : IGroup<TEntity> where TEntity : class, IEntity
    {
        /// Occurs when an entity gets added.
        public event GroupChanged<TEntity> OnEntityAdded;

        /// Occurs when an entity gets removed.
        public event GroupChanged<TEntity> OnEntityRemoved;

        /// Occurs when a component of an entity in the group gets replaced.
        public event GroupUpdated<TEntity> OnEntityUpdated;

        /// Returns the number of entities in the group.
        public int Count => _entities.Count;

        /// Returns the matcher which was used to create this group.
        public IMatcher<TEntity> Matcher => _matcher;

        readonly IMatcher<TEntity> _matcher;
        readonly HashSet<TEntity> _entities = new HashSet<TEntity>(EntityEqualityComparer<TEntity>.Comparer);

        TEntity[] _entitiesCache;
        TEntity _singleEntityCache;
        string _toStringCache;

        /// Use context.GetGroup(matcher) to get a group of entities which match
        /// the specified matcher.
        public Group(IMatcher<TEntity> matcher)
        {
            _matcher = matcher;
        }

        /// This is used by the context to manage the group.
        public void HandleEntitySilently(TEntity entity)
        {
            if (_matcher.Matches(entity))
                AddEntitySilently(entity);
            else
                RemoveEntitySilently(entity);
        }

        /// This is used by the context to manage the group.
        public void HandleEntity(TEntity entity, int index, IComponent component)
        {
            if (_matcher.Matches(entity))
                AddEntity(entity, index, component);
            else
                RemoveEntity(entity, index, component);
        }

        /// This is used by the context to manage the group.
        public void UpdateEntity(TEntity entity, int index, IComponent previousComponent, IComponent newComponent)
        {
            if (_entities.Contains(entity))
            {
                OnEntityRemoved?.Invoke(this, entity, index, previousComponent);
                OnEntityAdded?.Invoke(this, entity, index, newComponent);
                OnEntityUpdated?.Invoke(this, entity, index, previousComponent, newComponent);
            }
        }

        /// Removes all event handlers from this group.
        /// Keep in mind that this will break reactive systems and
        /// entity indexes which rely on this group.
        public void RemoveAllEventHandlers()
        {
            OnEntityAdded = null;
            OnEntityRemoved = null;
            OnEntityUpdated = null;
        }

        public GroupChanged<TEntity> HandleEntity(TEntity entity) =>
            _matcher.Matches(entity)
                ? AddEntitySilently(entity) ? OnEntityAdded : null
                : RemoveEntitySilently(entity)
                    ? OnEntityRemoved
                    : null;

        bool AddEntitySilently(TEntity entity)
        {
            if (entity.IsEnabled)
            {
                var added = _entities.Add(entity);
                if (added)
                {
                    _entitiesCache = null;
                    _singleEntityCache = null;
                    entity.Retain(this);
                }

                return added;
            }

            return false;
        }

        void AddEntity(TEntity entity, int index, IComponent component)
        {
            if (AddEntitySilently(entity))
                OnEntityAdded?.Invoke(this, entity, index, component);
        }

        bool RemoveEntitySilently(TEntity entity)
        {
            var removed = _entities.Remove(entity);
            if (removed)
            {
                _entitiesCache = null;
                _singleEntityCache = null;
                entity.Release(this);
            }

            return removed;
        }

        void RemoveEntity(TEntity entity, int index, IComponent component)
        {
            var removed = _entities.Remove(entity);
            if (removed)
            {
                _entitiesCache = null;
                _singleEntityCache = null;
                OnEntityRemoved?.Invoke(this, entity, index, component);
                entity.Release(this);
            }
        }

        /// Determines whether this group has the specified entity.
        public bool ContainsEntity(TEntity entity) => _entities.Contains(entity);

        /// Returns all entities which are currently in this group.
        public TEntity[] GetEntities()
        {
            return _entitiesCache ??= _entities.ToArray();
        }

        /// Fills the buffer with all entities which are currently in this group.
        public List<TEntity> GetEntities(List<TEntity> buffer)
        {
            buffer.Clear();
            buffer.AddRange(_entities);
            return buffer;
        }

        public IEnumerable<TEntity> AsEnumerable() => _entities;

        public HashSet<TEntity>.Enumerator GetEnumerator() => _entities.GetEnumerator();

        /// Returns the only entity in this group. It will return null
        /// if the group is empty. It will throw an exception if the group
        /// has more than one entity.
        public TEntity GetSingleEntity()
        {
            if (_singleEntityCache == null)
            {
                var c = _entities.Count;
                if (c == 1)
                {
                    using (var enumerator = _entities.GetEnumerator())
                    {
                        enumerator.MoveNext();
                        _singleEntityCache = enumerator.Current;
                    }
                }
                else if (c == 0)
                {
                    return null;
                }
                else
                {
                    throw new GroupSingleEntityException<TEntity>(this);
                }
            }

            return _singleEntityCache;
        }

        public override string ToString() => _toStringCache ??= $"Group({_matcher})";
    }
}
