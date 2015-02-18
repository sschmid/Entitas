using System.Collections.Generic;

namespace Entitas {
    public enum GroupEventType : byte {
        OnEntityAdded,
        OnEntityAddedStrict,
        OnEntityRemoved,
        OnEntityRemovedStrict,
        OnEntityAddedOrRemoved
    }

    public class GroupObserver {
        public HashSet<Entity> collectedEntities {
            get {
                filter();
                return _collectedEntities;
            }
        }

        readonly HashSet<Entity> _collectedEntities;
        readonly Group _group;
        readonly GroupEventType _eventType;

        public GroupObserver(Group group, GroupEventType eventType) {
            _collectedEntities = new HashSet<Entity>(EntityEqualityComparer.comparer);
            _group = group;
            _eventType = eventType;
            Activate();
        }

        public void Activate() {
            if (_eventType == GroupEventType.OnEntityAdded || _eventType == GroupEventType.OnEntityAddedStrict) {
                _group.OnEntityAdded += addEntity;
            } else if (_eventType == GroupEventType.OnEntityRemoved || _eventType == GroupEventType.OnEntityRemovedStrict) {
                _group.OnEntityRemoved += addEntity;
            } else if (_eventType == GroupEventType.OnEntityAddedOrRemoved) {
                _group.OnEntityAdded += addEntity;
                _group.OnEntityRemoved += addEntity;
            }
        }

        public void Deactivate() {
            _group.OnEntityAdded -= addEntity;
            _group.OnEntityRemoved -= addEntity;
            _collectedEntities.Clear();
        }

        public void ClearCollectedEntites() {
            _collectedEntities.Clear();
        }

        void addEntity(Group group, Entity entity) {
            _collectedEntities.Add(entity);
        }

        void filter() {
            if (_eventType == GroupEventType.OnEntityAddedStrict) {
                var entitiesToRemove = new List<Entity>();
                foreach (var entity in _collectedEntities) {
                    if (!_group.ContainsEntity(entity)) {
                        entitiesToRemove.Add(entity);
                    }
                }
                foreach (var entity in entitiesToRemove) {
                    _collectedEntities.Remove(entity);
                }
            } else if (_eventType == GroupEventType.OnEntityRemovedStrict) {
                var entitiesToRemove = new List<Entity>();
                foreach (var entity in _collectedEntities) {
                    if (_group.ContainsEntity(entity)) {
                        entitiesToRemove.Add(entity);
                    }
                }
                foreach (var entity in entitiesToRemove) {
                    _collectedEntities.Remove(entity);
                }
            }
        }
    }
}

