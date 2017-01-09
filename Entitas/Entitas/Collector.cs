using System.Collections.Generic;
using System.Text;

namespace Entitas {

    /// An Collector can observe one or more groups and collects
    /// changed entities based on the specified groupEvent.
    public class Collector {

        /// Returns all collected entities.
        /// Call collector.ClearCollectedEntities()
        /// once you processed all entities.
        public HashSet<Entity> collectedEntities {
            get { return _collectedEntities; }
        }

        readonly HashSet<Entity> _collectedEntities;
        readonly Group[] _groups;
        readonly GroupEvent[] _groupEvents;
        Group.GroupChanged _addEntityCache;
        string _toStringCache;
        StringBuilder _toStringBuilder;

        /// Creates an Collector and will collect changed entities
        /// based on the specified groupEvent.
        public Collector(Group group, GroupEvent groupEvent)
            : this(new [] { group }, new [] { groupEvent }) {
        }

        /// Creates an Collector and will collect changed entities
        /// based on the specified groupEvents.
        public Collector(Group[] groups, GroupEvent[] groupEvents) {
            _groups = groups;
            _collectedEntities = new HashSet<Entity>(
                EntityEqualityComparer.comparer
            );
            _groupEvents = groupEvents;

            if(groups.Length != groupEvents.Length) {
                throw new CollectorException(
                    "Unbalanced count with groups (" + groups.Length +
                    ") and group events (" + groupEvents.Length + ").",
                    "Group and group events count must be equal."
                );
            }

            _addEntityCache = addEntity;
            Activate();
        }

        /// Activates the Collector and will start collecting
        /// changed entities. Collectors are activated by default.
        public void Activate() {
            for (int i = 0; i < _groups.Length; i++) {
                var group = _groups[i];
                var groupEvent = _groupEvents[i];
                if(groupEvent == GroupEvent.Added) {
                    group.OnEntityAdded -= _addEntityCache;
                    group.OnEntityAdded += _addEntityCache;
                } else if(groupEvent == GroupEvent.Removed) {
                    group.OnEntityRemoved -= _addEntityCache;
                    group.OnEntityRemoved += _addEntityCache;
                } else if(groupEvent == GroupEvent.AddedOrRemoved) {
                    group.OnEntityAdded -= _addEntityCache;
                    group.OnEntityAdded += _addEntityCache;
                    group.OnEntityRemoved -= _addEntityCache;
                    group.OnEntityRemoved += _addEntityCache;
                }
            }
        }

        /// Deactivates the Collector.
        /// This will also clear all collected entities.
        /// Collectors are activated by default.
        public void Deactivate() {
            for (int i = 0; i < _groups.Length; i++) {
                var group = _groups[i];
                group.OnEntityAdded -= _addEntityCache;
                group.OnEntityRemoved -= _addEntityCache;
            }
            ClearCollectedEntities();
        }

        /// Clears all collected entities.
        public void ClearCollectedEntities() {
            foreach(var entity in _collectedEntities) {
                entity.Release(this);
            }
            _collectedEntities.Clear();
        }

        void addEntity(Group group,
                       Entity entity,
                       int index,
                       IComponent component) {
            var added = _collectedEntities.Add(entity);
            if(added) {
                entity.Retain(this);
            }
        }

        public override string ToString() {
            if(_toStringCache == null) {
                if(_toStringBuilder == null) {
                    _toStringBuilder = new StringBuilder();
                }
                _toStringBuilder.Length = 0;
                _toStringBuilder.Append("Collector(");

                const string separator = ", ";
                var lastSeparator = _groups.Length - 1;
                for (int i = 0; i < _groups.Length; i++) {
                    _toStringBuilder.Append(_groups[i]);
                    if(i < lastSeparator) {
                        _toStringBuilder.Append(separator);
                    }
                }

                _toStringBuilder.Append(")");
                _toStringCache = _toStringBuilder.ToString();
            }

            return _toStringCache;
        }

        ~Collector () {
            Deactivate();
        }
    }

    public class CollectorException : EntitasException {
        public CollectorException(string message, string hint) :
            base(message, hint) {
        }
    }
}
