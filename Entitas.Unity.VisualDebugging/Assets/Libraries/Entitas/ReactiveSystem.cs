using System.Collections.Generic;

namespace Entitas {
    public class ReactiveSystem : IExecuteSystem {
        public IReactiveSystem subsystem { get { return _subsystem; } }

        readonly IReactiveSystem _subsystem;
        readonly GroupObserver _observer;
        readonly List<Entity> _buffer;

        public ReactiveSystem(Pool pool, IReactiveSystem subSystem) {
            _subsystem = subSystem;
            _observer = new GroupObserver(pool.GetGroup(subSystem.trigger), subSystem.eventType);
            _buffer = new List<Entity>();
        }

        public void Activate() {
            _observer.Activate();
        }

        public void Deactivate() {
            _observer.Deactivate();
        }

        public void Execute() {
            if (_observer.collectedEntities.Count != 0) {
                _buffer.AddRange(_observer.collectedEntities);
                _observer.ClearCollectedEntities();
                _subsystem.Execute(_buffer);
                _buffer.Clear();
            }
        }
    }
}

