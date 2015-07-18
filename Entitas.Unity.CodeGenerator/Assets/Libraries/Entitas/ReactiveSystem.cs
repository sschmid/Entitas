namespace Entitas {
    public class ReactiveSystem : IExecuteSystem {
        public IReactiveSystem subsystem { get { return _subsystem; } }

        readonly IReactiveSystem _subsystem;
        readonly GroupObserver _observer;

        public ReactiveSystem(Pool pool, IReactiveSystem subSystem) {
            _subsystem = subSystem;
            _observer = new GroupObserver(pool.GetGroup(subSystem.trigger), subSystem.eventType);
        }

        public void Activate() {
            _observer.Activate();
        }

        public void Deactivate() {
            _observer.Deactivate();
        }

        public void Execute() {
            if (_observer.collectedEntities.Count != 0) {
                var entities = new Entity[_observer.collectedEntities.Count];
                _observer.collectedEntities.CopyTo(entities, 0);
                _observer.ClearCollectedEntities();
                _subsystem.Execute(entities);
            }
        }
    }
}

