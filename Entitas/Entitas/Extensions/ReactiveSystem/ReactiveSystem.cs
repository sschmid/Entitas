namespace Entitas {
    public class ReactiveSystem : IExecuteSystem {
        public IReactiveSystem subsystem { get { return _subsystem; } }

        readonly IReactiveSystem _subsystem;
        readonly GroupObserver _observer;

        public ReactiveSystem(Pool pool, IReactiveSystem subSystem) {
            _subsystem = subSystem;
            _observer = new GroupObserver(pool.GetGroup(subSystem.GetTriggeringMatcher()), subSystem.GetEventType());
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

