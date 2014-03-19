using ToolKit;

namespace Entitas {
    public class ReactiveEntitySystem : IEntitySystem {
        readonly IReactiveSubEntitySystem _subsystem;
        readonly OrderedSet<Entity> _collectedEntites;

        public ReactiveEntitySystem(EntityRepository repo, IReactiveSubEntitySystem subSystem) {
            _subsystem = subSystem;
            _collectedEntites = new OrderedSet<Entity>();

            var collection = repo.GetCollection(subSystem.GetTriggeringMatcher());
            var eventType = subSystem.GetEventType();
            if (eventType == EntityCollectionEventType.OnEntityAdded) {
                collection.OnEntityAdded += addEntity;
            } else if (eventType == EntityCollectionEventType.OnEntityRemoved) {
                collection.OnEntityRemoved += addEntity;
            } else if (eventType == EntityCollectionEventType.OnEntityAddedSafe) {
                collection.OnEntityAdded += addEntity;
                collection.OnEntityRemoved += removeEntity;
            }
        }

        void addEntity(EntityCollection collection, Entity entity) {
            _collectedEntites.Add(entity);
        }

        void removeEntity(EntityCollection collection, Entity entity) {
            _collectedEntites.Remove(entity);
        }

        public void Execute() {
            if (_collectedEntites.Count > 0) {
                _subsystem.Execute(_collectedEntites.ToArray());
                _collectedEntites.Clear();
            }
        }
    }
}

