using ToolKit;
using System.Collections.Generic;

namespace Entitas {
    public class EntityRepositoryObserver {
        public List<Entity> collectedEntites { get { return _collectedEntites.list; } }

        readonly ListSet<Entity> _collectedEntites;
        readonly EntityCollection _collection;
        readonly EntityCollectionEventType _eventType;

        public EntityRepositoryObserver(EntityRepository repo, EntityCollectionEventType eventType, IEntityMatcher matcher) {
            _collectedEntites = new ListSet<Entity>();
            _collection = repo.GetCollection(matcher);
            _eventType = eventType;
            Activate();
        }

        public void Activate() {
            if (_eventType == EntityCollectionEventType.OnEntityAdded)
                _collection.OnEntityAdded += addEntity;
            else if (_eventType == EntityCollectionEventType.OnEntityRemoved)
                _collection.OnEntityRemoved += addEntity;
            else if (_eventType == EntityCollectionEventType.OnEntityAddedOrRemoved) {
                _collection.OnEntityAdded += addEntity;
                _collection.OnEntityRemoved += addEntity;
            }
        }

        public void Deactivate() {
            _collectedEntites.Clear();
            _collection.OnEntityAdded -= addEntity;
            _collection.OnEntityRemoved -= addEntity;
        }

        public void ClearCollectedEntites() {
            _collectedEntites.Clear();
        }

        void addEntity(EntityCollection collection, Entity entity) {
            _collectedEntites.Add(entity);
        }
    }
}

