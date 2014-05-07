namespace Entitas {
    public class ReactiveEntitySystem : IEntitySystem {
        public IReactiveSubEntitySystem subsystem { get { return _subsystem; } }

        readonly IReactiveSubEntitySystem _subsystem;
        readonly EntityRepositoryObserver _observer;

        public ReactiveEntitySystem(EntityRepository repo, IReactiveSubEntitySystem subSystem) {
            _subsystem = subSystem;
            _observer = new EntityRepositoryObserver(repo, subSystem.GetEventType(), subSystem.GetTriggeringMatcher());
        }

        public void Execute() {
            var entities = _observer.collectedEntites;
            if (entities.Count > 0) {
                _subsystem.Execute(entities);
                _observer.ClearCollectedEntites();
            }
        }
    }
}

