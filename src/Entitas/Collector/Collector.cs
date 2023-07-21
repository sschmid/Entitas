using System.Collections.Generic;
using System.Linq;

namespace Entitas
{
    /// A Collector can observe one or more groups from the same context
    /// and collects changed entities based on the specified groupEvent.
    public class Collector<TEntity> : ICollector<TEntity> where TEntity : class, IEntity
    {
        /// Returns all collected entities.
        /// Call collector.ClearCollectedEntities()
        /// once you processed all entities.
        public HashSet<TEntity> CollectedEntities => _collectedEntities;

        /// Returns the number of all collected entities.
        public int Count => _collectedEntities.Count;

        readonly HashSet<TEntity> _collectedEntities;
        readonly IGroup<TEntity>[] _groups;
        readonly GroupEvent[] _groupEvents;

        // Cache delegate to reduce gc allocations
        readonly GroupChanged<TEntity> _onEntityDelegate;

        string _toStringCache;

        /// Creates a Collector and will collect changed entities
        /// based on the specified groupEvent.
        public Collector(IGroup<TEntity> group, GroupEvent groupEvent) : this(new[] { group }, new[] { groupEvent }) { }

        /// Creates a Collector and will collect changed entities
        /// based on the specified groupEvents.
        public Collector(IGroup<TEntity>[] groups, GroupEvent[] groupEvents)
        {
            _collectedEntities = new HashSet<TEntity>(EntityEqualityComparer<TEntity>.Comparer);
            _groups = groups;
            _groupEvents = groupEvents;

            if (groups.Length != groupEvents.Length)
            {
                throw new CollectorException(
                    $"Unbalanced count with groups ({groups.Length}) and group events ({groupEvents.Length}).",
                    "Group and GroupEvents count must be equal."
                );
            }

            _onEntityDelegate = (_, entity, _, _) =>
            {
                if (_collectedEntities.Add(entity))
                    entity.Retain(this);
            };

            Activate();
        }

        /// Activates the Collector and will start collecting
        /// changed entities. Collectors are activated by default.
        public void Activate()
        {
            for (var i = 0; i < _groups.Length; i++)
            {
                var group = _groups[i];
                var groupEvent = _groupEvents[i];
                switch (groupEvent)
                {
                    case GroupEvent.Added:
                        group.OnEntityAdded -= _onEntityDelegate;
                        group.OnEntityAdded += _onEntityDelegate;
                        break;
                    case GroupEvent.Removed:
                        group.OnEntityRemoved -= _onEntityDelegate;
                        group.OnEntityRemoved += _onEntityDelegate;
                        break;
                    case GroupEvent.AddedOrRemoved:
                        group.OnEntityAdded -= _onEntityDelegate;
                        group.OnEntityAdded += _onEntityDelegate;
                        group.OnEntityRemoved -= _onEntityDelegate;
                        group.OnEntityRemoved += _onEntityDelegate;
                        break;
                }
            }
        }

        /// Deactivates the Collector.
        /// This will also clear all collected entities.
        /// Collectors are activated by default.
        public void Deactivate()
        {
            for (var i = 0; i < _groups.Length; i++)
            {
                var group = _groups[i];
                group.OnEntityAdded -= _onEntityDelegate;
                group.OnEntityRemoved -= _onEntityDelegate;
            }

            ClearCollectedEntities();
        }

        /// Clears all collected entities.
        public void ClearCollectedEntities()
        {
            foreach (var entity in _collectedEntities)
                entity.Release(this);

            _collectedEntities.Clear();
        }

        public override string ToString()
        {
            return _toStringCache ??= "Collector(" + string.Join(", ", _groups.Select(group => group.ToString())) + ")";
        }

        ~Collector() => Deactivate();
    }
}
