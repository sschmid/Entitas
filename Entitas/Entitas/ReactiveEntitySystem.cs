using ToolKit;

namespace Entitas {
    public class ReactiveEntitySystem : IEntitySystem {
        public IReactiveSubEntitySystem subsystem { get { return _subsystem; } }

        readonly IReactiveSubEntitySystem _subsystem;
        readonly ListSet<Entity> _collectedEntites;

        public ReactiveEntitySystem(EntityRepository repo, IReactiveSubEntitySystem subSystem) {
            _subsystem = subSystem;
            _collectedEntites = new ListSet<Entity>();

            var collection = repo.GetCollection(subSystem.GetTriggeringMatcher());
            var eventType = subSystem.GetEventType();
            if (eventType == EntityCollectionEventType.OnEntityAdded)
                collection.OnEntityAdded += addEntity;
            else if (eventType == EntityCollectionEventType.OnEntityRemoved)
                collection.OnEntityRemoved += addEntity;
        }

        void addEntity(EntityCollection collection, Entity entity) {
            _collectedEntites.Add(entity);
        }

        void removeEntity(EntityCollection collection, Entity entity) {
            _collectedEntites.Remove(entity);
        }

        public void Execute() {
            if (_collectedEntites.Count > 0) {
                _subsystem.Execute(_collectedEntites.list);
                _collectedEntites.Clear();
            }
        }
    }
}

