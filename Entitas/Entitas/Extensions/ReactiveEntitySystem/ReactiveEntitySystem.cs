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
            for (int i = 0, entitesCount = _observer.collectedEntites.Count; i < entitesCount; i++)
                _buffer.Add(_observer.collectedEntites[i]);
            _observer.ClearCollectedEntites();

            if (_buffer.Count > 0)
                _subsystem.Execute(_buffer);
            _buffer.Clear();
        }
    }
}

