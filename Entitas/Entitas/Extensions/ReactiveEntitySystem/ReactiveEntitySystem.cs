namespace Entitas {
    public class ReactiveEntitySystem : IEntitySystem {
        public IReactiveSubEntitySystem subsystem { get { return _subsystem; } }

        readonly IReactiveSubEntitySystem _subsystem;
        readonly EntityRepositoryObserver _observer;

        public ReactiveEntitySystem(EntityRepository repo, IReactiveSubEntitySystem subSystem) {
            _subsystem = subSystem;
            _observer = new EntityRepositoryObserver(repo, subSystem.GetTriggeringMatcher(), subSystem.GetEventType());
        }

        public void Execute() {
            var buffer = new Entity[_observer.collectedEntities.Count];
            _observer.collectedEntities.CopyTo(buffer, 0);
            _observer.ClearCollectedEntites();
            if (buffer.Length > 0) {
                _subsystem.Execute(buffer);
            }
        }
    }
}

