using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Entitas {

    /// <summary>
    /// A Collector can observe one or more groups from the same context
    /// and collects changed entities based on the specified groupEvent.
    /// </summary>
    public class Collector<TEntity> : ICollector<TEntity> where TEntity : class, IEntity {

        /// <summary>
        /// Returns all collected entities.
        /// Call collector.ClearCollectedEntities()
        /// once you processed all entities.
        /// </summary>
        public HashSet<TEntity> collectedEntities { get { return _collectedEntities; } }

        /// <summary>
        /// Returns the number of all collected entities.
        /// </summary>
        public int count { get { return _collectedEntities.Count; } }

        readonly HashSet<TEntity> _collectedEntities;
        readonly IGroup<TEntity>[] _groups;
        readonly GroupEvent[] _groupEvents;

        GroupChanged<TEntity> _addEntityCache;
        string _toStringCache;
        StringBuilder _toStringBuilder;

        /// <summary>
        /// Creates a Collector and will collect changed entities
        /// based on the specified groupEvent.
        /// </summary>
        public Collector(IGroup<TEntity> group, GroupEvent groupEvent) : this(new[] { group }, new[] { groupEvent }) {
        }

        /// <summary>
        /// Creates a Collector and will collect changed entities
        /// based on the specified groupEvents.
        /// </summary>
        public Collector(IGroup<TEntity>[] groups, GroupEvent[] groupEvents) {
            _groups = groups;
            _collectedEntities = new HashSet<TEntity>(EntityEqualityComparer<TEntity>.comparer);
            _groupEvents = groupEvents;

            if (groups.Length != groupEvents.Length) {
                throw new CollectorException(
                    "Unbalanced count with groups (" + groups.Length +
                    ") and group events (" + groupEvents.Length + ").",
                    "Group and group events count must be equal."
                );
            }

            _addEntityCache = addEntity;
            Activate();
        }

        /// <summary>
        /// Activates the Collector and will start collecting
        /// changed entities. Collectors are activated by default.
        /// </summary>
        public void Activate() {
            for (int i = 0; i < _groups.Length; i++) {
                var group = _groups[i];
                var groupEvent = _groupEvents[i];
                switch (groupEvent) {
                    case GroupEvent.Added:
                        group.OnEntityAdded -= _addEntityCache;
                        group.OnEntityAdded += _addEntityCache;
                        break;
                    case GroupEvent.Removed:
                        group.OnEntityRemoved -= _addEntityCache;
                        group.OnEntityRemoved += _addEntityCache;
                        break;
                    case GroupEvent.AddedOrRemoved:
                        group.OnEntityAdded -= _addEntityCache;
                        group.OnEntityAdded += _addEntityCache;
                        group.OnEntityRemoved -= _addEntityCache;
                        group.OnEntityRemoved += _addEntityCache;
                        break;
                }
            }
        }

        /// <summary>
        /// Deactivates the Collector.
        /// This will also clear all collected entities.
        /// Collectors are activated by default.
        /// </summary>
        public void Deactivate() {
            for (int i = 0; i < _groups.Length; i++) {
                var group = _groups[i];
                group.OnEntityAdded -= _addEntityCache;
                group.OnEntityRemoved -= _addEntityCache;
            }
            ClearCollectedEntities();
        }

        /// <summary>
        /// Returns all collected entities and casts them.
        /// Call collector.ClearCollectedEntities()
        /// once you processed all entities.
        /// </summary>
        public IEnumerable<TCast> GetCollectedEntities<TCast>() where TCast : class, IEntity {
            return _collectedEntities.Cast<TCast>();
        }

        /// <summary>
        /// Clears all collected entities.
        /// </summary>
        public void ClearCollectedEntities() {
            foreach (var entity in _collectedEntities) {
                entity.Release(this);
            }
            _collectedEntities.Clear();
        }

        void addEntity(IGroup<TEntity> group, TEntity entity, int index, IComponent component) {
            var added = _collectedEntities.Add(entity);
            if (added) {
                entity.Retain(this);
            }
        }

        public override string ToString() {
            if (_toStringCache == null) {
                if (_toStringBuilder == null) {
                    _toStringBuilder = new StringBuilder();
                }
                _toStringBuilder.Length = 0;
                _toStringBuilder.Append("Collector(");

                const string separator = ", ";
                var lastSeparator = _groups.Length - 1;
                for (int i = 0; i < _groups.Length; i++) {
                    _toStringBuilder.Append(_groups[i]);
                    if (i < lastSeparator) {
                        _toStringBuilder.Append(separator);
                    }
                }

                _toStringBuilder.Append(")");
                _toStringCache = _toStringBuilder.ToString();
            }

            return _toStringCache;
        }

        ~Collector() {
            Deactivate();
        }
    }
}
