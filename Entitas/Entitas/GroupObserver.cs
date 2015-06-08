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
        readonly Group _group;
        readonly GroupEventType _eventType;

        public GroupObserver(Group group, GroupEventType eventType) {
            _collectedEntities = new HashSet<Entity>(EntityEqualityComparer.comparer);
            _group = group;
            _eventType = eventType;
            Activate();
        }

        public void Activate() {
            if (_eventType == GroupEventType.OnEntityAdded) {
                _group.OnEntityAdded += addEntity;
            } else if (_eventType == GroupEventType.OnEntityRemoved) {
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

        public void ClearCollectedEntities() {
            _collectedEntities.Clear();
        }

        void addEntity(Group group, Entity entity) {
            _collectedEntities.Add(entity);
        }
    }
}

