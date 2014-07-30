using System.Collections.Generic;

namespace Entitas {
    public class ReactiveEntityWillBeRemovedSystem : IEntitySystem {
        public IReactiveSubEntityWillBeRemovedSystem subsystem { get { return _subsystem; } }

        readonly IReactiveSubEntityWillBeRemovedSystem _subsystem;
        readonly EntityWillBeRemovedEntityRepositoryObserver _observer;
        readonly List<EntityComponentPair> _buffer = new List<EntityComponentPair>();

        public ReactiveEntityWillBeRemovedSystem(EntityRepository repo, IReactiveSubEntityWillBeRemovedSystem subSystem) {
            _subsystem = subSystem;
            _observer = new EntityWillBeRemovedEntityRepositoryObserver(repo, subSystem.GetTriggeringIndex());
        }

        public void Execute() {
            _buffer.AddRange(_observer.collectedEntityComponentPairs);
            _observer.ClearCollectedEntites();
            if (_buffer.Count > 0)
                _subsystem.Execute(_buffer);
            _buffer.Clear();
        }
    }
}

