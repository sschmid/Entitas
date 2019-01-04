using System.Collections.Generic;

namespace Entitas {

    /// <summary>
    /// A ReactiveSystem calls Execute(entities) if there were changes based on
    /// the specified Collector and will only pass in changed entities.
    /// A common use-case is to react to changes, e.g. a change of the position
    /// of an entity to update the gameObject.transform.position
    /// of the related gameObject.
    /// </summary>
    public abstract class MultiReactiveSystem<TEntity, TContexts> : IReactiveSystem
        where TEntity : class, IEntity
        where TContexts : class, IContexts {

        readonly ICollector[] _collectors;
        readonly HashSet<TEntity> _collectedEntities;
        readonly List<TEntity> _buffer;
        string _toStringCache;

        protected MultiReactiveSystem(TContexts contexts) {
            _collectors = GetTrigger(contexts);
            _collectedEntities = new HashSet<TEntity>();
            _buffer = new List<TEntity>();
        }

        protected MultiReactiveSystem(ICollector[] collectors) {
            _collectors = collectors;
            _collectedEntities = new HashSet<TEntity>();
            _buffer = new List<TEntity>();
        }

        /// <summary>
        /// Specify the collector that will trigger the ReactiveSystem.
        /// </summary>
        protected abstract ICollector[] GetTrigger(TContexts contexts);

        /// <summary>
        /// This will exclude all entities which don't pass the filter.
        /// </summary>
        protected abstract bool Filter(TEntity entity);

        protected abstract void Execute(List<TEntity> entities);

        /// <summary>
        /// Activates the ReactiveSystem and starts observing changes
        /// based on the specified Collector.
        /// ReactiveSystem are activated by default.
        /// </summary>
        public void Activate() {
            for (int i = 0; i < _collectors.Length; i++) {
                _collectors[i].Activate();
            }
        }

        /// <summary>
        /// Deactivates the ReactiveSystem.
        /// No changes will be tracked while deactivated.
        /// This will also clear the ReactiveSystem.
        /// ReactiveSystem are activated by default.
        /// </summary>
        public void Deactivate() {
            for (int i = 0; i < _collectors.Length; i++) {
                _collectors[i].Deactivate();
            }
        }

        /// <summary>
        /// Clears all accumulated changes.
        /// </summary>
        public void Clear() {
            for (int i = 0; i < _collectors.Length; i++) {
                _collectors[i].ClearCollectedEntities();
            }
        }

        /// <summary>
        /// Will call Execute(entities) with changed entities
        /// if there are any. Otherwise it will not call Execute(entities).
        /// </summary>
        public void Execute() {
            for (int i = 0; i < _collectors.Length; i++) {
                var collector = _collectors[i];
                if (collector.count != 0) {
                    _collectedEntities.UnionWith(collector.GetCollectedEntities<TEntity>());
                    collector.ClearCollectedEntities();
                }
            }

            foreach (var e in _collectedEntities) {
                if (Filter(e)) {
                    e.Retain(this);
                    _buffer.Add(e);
                }
            }

            if (_buffer.Count != 0) {
                try {
                    Execute(_buffer);
                } finally {
                    for (int i = 0; i < _buffer.Count; i++) {
                        _buffer[i].Release(this);
                    }
                    _collectedEntities.Clear();
                    _buffer.Clear();
                }
            }
        }

        public override string ToString() {
            if (_toStringCache == null) {
                _toStringCache = "MultiReactiveSystem(" + GetType().Name + ")";
            }

            return _toStringCache;
        }

        ~MultiReactiveSystem() {
            Deactivate();
        }
    }
}
