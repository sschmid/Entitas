using System.Collections.Generic;

namespace Entitas {

    /// <summary>
    /// A ReactiveSystem calls Execute(entities) if there were changes based on
    /// the specified Collector and will only pass in changed entities.
    /// A common use-case is to react to changes, e.g. a change of the position
    /// of an entity to update the gameObject.transform.position
    /// of the related gameObject.
    /// </summary>
    public abstract class ReactiveSystem<TEntity> : IReactiveSystem where TEntity : class, IEntity {

        readonly ICollector<TEntity> _collector;
        readonly List<TEntity> _buffer;
        string _toStringCache;

        protected ReactiveSystem(IContext<TEntity> context) {
            _collector = GetTrigger(context);
            _buffer = new List<TEntity>();
        }

        protected ReactiveSystem(ICollector<TEntity> collector) {
            _collector = collector;
            _buffer = new List<TEntity>();
        }

        /// <summary>
        /// Specify the collector that will trigger the ReactiveSystem.
        /// </summary>
        protected abstract ICollector<TEntity> GetTrigger(IContext<TEntity> context);

        /// <summary>
        /// This will exclude all entities which don't pass the filter.
        /// </summary>
        protected abstract bool Filter(TEntity entity);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        protected abstract void Execute(List<TEntity> entities);

        /// <summary>
        /// Activates the ReactiveSystem and starts observing changes
        /// based on the specified Collector.
        /// ReactiveSystem are activated by default.
        /// </summary>
        public void Activate() {
            _collector.Activate();
        }

        /// <summary>
        /// Deactivates the ReactiveSystem.
        /// No changes will be tracked while deactivated.
        /// This will also clear the ReactiveSystem.
        /// ReactiveSystem are activated by default.
        /// </summary>
        public void Deactivate() {
            _collector.Deactivate();
        }

        /// <summary>
        /// Clears all accumulated changes.
        /// </summary>
        public void Clear() {
            _collector.ClearCollectedEntities();
        }

        /// <summary>
        /// Will call Execute(entities) with changed entities
        /// if there are any. Otherwise it will not call Execute(entities).
        /// </summary>
        public void Execute() {
            if (_collector.count != 0) {
                foreach (var e in _collector.collectedEntities) {
                    if (Filter(e)) {
                        e.Retain(this);
                        _buffer.Add(e);
                    }
                }

                _collector.ClearCollectedEntities();

                if (_buffer.Count != 0) {
                    try {
                        Execute(_buffer);
                    } finally {
                        for (int i = 0; i < _buffer.Count; i++) {
                            _buffer[i].Release(this);
                        }
                        _buffer.Clear();
                    }
                }
            }
        }

        public override string ToString() {
            if (_toStringCache == null) {
                _toStringCache = "ReactiveSystem(" + GetType().Name + ")";
            }

            return _toStringCache;
        }

        ~ReactiveSystem() {
            Deactivate();
        }
    }
}
