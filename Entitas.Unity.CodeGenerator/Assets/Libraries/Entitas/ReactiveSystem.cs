using System.Collections.Generic;

namespace Entitas {
    public class ReactiveSystem : IExecuteSystem {
        public IReactiveExecuteSystem subsystem { get { return _subsystem; } }

        readonly IReactiveExecuteSystem _subsystem;
        readonly GroupObserver _observer;
        readonly List<Entity> _buffer;

        public ReactiveSystem(Pool pool, IReactiveSystem subSystem) :
            this(pool, subSystem, new [] { subSystem.trigger }, new [] { subSystem.eventType }) {
        }

        public ReactiveSystem(Pool pool, IMultiReactiveSystem subSystem) :
            this(pool, subSystem, subSystem.triggers, subSystem.eventTypes) {
        }

        ReactiveSystem(Pool pool, IReactiveExecuteSystem subSystem, IMatcher[] triggers, GroupEventType[] eventTypes) {
            _subsystem = subSystem;
            var groups = new Group[triggers.Length];
            for (int i = 0, triggersLength = triggers.Length; i < triggersLength; i++) {
                groups[i] = pool.GetGroup(triggers[i]);
            }
            _observer = new GroupObserver(groups, eventTypes);
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

