namespace Entitas {
    public class ReactiveWillBeRemovedSystem : IExecuteSystem {
        public IReactiveWillBeRemovedSystem subsystem { get { return _subsystem; } }

        readonly IReactiveWillBeRemovedSystem _subsystem;
        readonly WillBeRemovedContextObserver _observer;

        public ReactiveWillBeRemovedSystem(Context context, IReactiveWillBeRemovedSystem subSystem) {
            _subsystem = subSystem;
            _observer = new WillBeRemovedContextObserver(context, subSystem.GetTriggeringMatcher());
        }

        public void Execute() {
            var buffer = _observer.collectedEntityComponentPairs.ToArray();
            _observer.ClearCollectedEntites();
            if (buffer.Length > 0) {
                _subsystem.Execute(buffer);
            }
        }
    }
}

