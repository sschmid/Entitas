using System.Collections.Generic;

namespace Entitas {
    public enum EntityCollectionEventType : byte {
        OnEntityAdded,
        OnEntityRemoved,
        OnEntityAddedOrRemoved
    }

    public class EntityRepositoryObserver {
        public HashSet<Entity> collectedEntities { get { return _collectedEntities; } }

        readonly HashSet<Entity> _collectedEntities;
        readonly EntityCollection _collection;
        readonly EntityCollectionEventType _eventType;

        public EntityRepositoryObserver(EntityRepository repo, IEntityMatcher matcher, EntityCollectionEventType eventType) {
            _collectedEntities = new HashSet<Entity>(EntityEqualityComparer.comparer);
            _collection = repo.GetCollection(matcher);
            _eventType = eventType;
            Activate();
        }

        public void Activate() {
            if (_eventType == EntityCollectionEventType.OnEntityAdded) {
                _collection.OnEntityAdded += addEntity;
            } else if (_eventType == EntityCollectionEventType.OnEntityRemoved) {
                _collection.OnEntityRemoved += addEntity;
            } else if (_eventType == EntityCollectionEventType.OnEntityAddedOrRemoved) {
                _collection.OnEntityAdded += addEntity;
                _collection.OnEntityRemoved += addEntity;
            }
        }

        public void Deactivate() {
            _collection.OnEntityAdded -= addEntity;
            _collection.OnEntityRemoved -= addEntity;
            _collectedEntities.Clear();
        }

        public void ClearCollectedEntites() {
            _collectedEntities.Clear();
        }

        void addEntity(EntityCollection collection, Entity entity) {
            _collectedEntities.Add(entity);
        }
    }
}

