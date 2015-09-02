using System;
using System.Collections.Generic;

namespace Entitas {
    public enum GroupEventType : byte {
        OnEntityAdded,
        OnEntityRemoved,
        OnEntityAddedOrRemoved
    }

    public class GroupObserver {
        public HashSet<Entity> collectedEntities { get { return _collectedEntities; } }

        readonly HashSet<Entity> _collectedEntities;
        readonly Group[] _groups;
        readonly GroupEventType[] _eventTypes;
        Group.GroupChanged _addEntityCache;

        public GroupObserver(Group group, GroupEventType eventType)
            : this(new [] { group }, new [] { eventType }) {
        }

        public GroupObserver(Group[] groups, GroupEventType[] eventTypes) {
            if (groups.Length != eventTypes.Length) {
                throw new GroupObserverException("Unbalanced count with groups (" + groups.Length +
                    ") and event types (" + eventTypes.Length + ")");
            }

            _collectedEntities = new HashSet<Entity>(EntityEqualityComparer.comparer);
            _groups = groups;
            _eventTypes = eventTypes;
            _addEntityCache = addEntity;
            Activate();
        }

        public void Activate() {
            for (int i = 0, groupsLength = _groups.Length; i < groupsLength; i++) {
                var group = _groups[i];
                var eventType = _eventTypes[i];
                if (eventType == GroupEventType.OnEntityAdded) {
                    group.OnEntityAdded -= _addEntityCache;
                    group.OnEntityAdded += _addEntityCache;
                } else if (eventType == GroupEventType.OnEntityRemoved) {
                    group.OnEntityRemoved -= _addEntityCache;
                    group.OnEntityRemoved += _addEntityCache;
                } else if (eventType == GroupEventType.OnEntityAddedOrRemoved) {
                    group.OnEntityAdded -= _addEntityCache;
                    group.OnEntityAdded += _addEntityCache;
                    group.OnEntityRemoved -= _addEntityCache;
                    group.OnEntityRemoved += _addEntityCache;
                }
            }
        }

        public void Deactivate() {
            for (int i = 0, groupsLength = _groups.Length; i < groupsLength; i++) {
                var group = _groups[i];
                group.OnEntityAdded -= _addEntityCache;
                group.OnEntityRemoved -= _addEntityCache;
            }
            ClearCollectedEntities();
        }

        public void ClearCollectedEntities() {
            foreach (var entity in _collectedEntities) {
                entity.Release();
            }
            _collectedEntities.Clear();
        }

        void addEntity(Group group, Entity entity, int index, IComponent component) {
            var added = _collectedEntities.Add(entity);
            if (added) {
                entity.Retain();
            }
        }
    }

    public class GroupObserverException : Exception {
        public GroupObserverException(string message) : base(message) {
        }
    }
}

