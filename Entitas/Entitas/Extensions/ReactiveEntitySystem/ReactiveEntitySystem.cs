using System.Collections.Generic;

namespace Entitas {
    public class ReactiveEntitySystem : IEntitySystem {
        public IReactiveSubEntitySystem subsystem { get { return _subsystem; } }

        readonly IReactiveSubEntitySystem _subsystem;
        readonly EntityRepositoryObserver _observer;
        readonly List<Entity> _buffer = new List<Entity>();

        public ReactiveEntitySystem(EntityRepository repo, IReactiveSubEntitySystem subSystem) {
            _subsystem = subSystem;
            _observer = new EntityRepositoryObserver(repo, subSystem.GetEventType(), subSystem.GetTriggeringMatcher());
        }

        public void Execute() {
            _buffer.AddRange(_observer.collectedEntities);
            _observer.ClearCollectedEntites();
            if (_buffer.Count > 0) {
                _subsystem.Execute(_buffer);
            }
            _buffer.Clear();
        }
    }
}

