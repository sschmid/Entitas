using System.Collections.Generic;

namespace Entitas {

    /// A ReactiveSystem calls Execute(entities) if there were changes based on
    /// the specified Collector and will only pass in changed entities.
    /// A common use-case is to react to changes, e.g. a change of the position
    /// of an entity to update the gameObject.transform.position
    /// of the related gameObject.
    public abstract class ReactiveSystem : IExecuteSystem {

        readonly Collector _collector;
        readonly List<IEntity> _buffer;
        string _toStringCache;

        protected ReactiveSystem(Context context) {
            _collector = GetTrigger(context);
            _buffer = new List<IEntity>();
        }

        protected ReactiveSystem(Collector collector) {
            _collector = collector;
            _buffer = new List<IEntity>();
        }

        /// Specify the collector that will trigger the ReactiveSystem.
        protected abstract Collector GetTrigger(Context context);

        /// This will exclude all entities which don't pass the filter.
        protected abstract bool Filter(IEntity entity);

        protected abstract void Execute(List<IEntity> entities);

        /// Activates the ReactiveSystem and starts observing changes
        /// based on the specified Collector.
        /// ReactiveSystem are activated by default.
        public void Activate() {
            _collector.Activate();
        }

        /// Deactivates the ReactiveSystem.
        /// No changes will be tracked while deactivated.
        /// This will also clear the ReactiveSystem.
        /// ReactiveSystem are activated by default.
        public void Deactivate() {
            _collector.Deactivate();
        }

        /// Clears all accumulated changes.
        public void Clear() {
            _collector.ClearCollectedEntities();
        }

        /// Will call Execute(entities) with changed entities
        /// if there are any. Otherwise it will not call Execute(entities).
        public void Execute() {
            if(_collector.collectedEntities.Count != 0) {
                foreach(var e in _collector.collectedEntities) {
                    if(Filter(e)) {
                        e.Retain(this);
                        _buffer.Add(e);
                    }
                }

                _collector.ClearCollectedEntities();

                if(_buffer.Count != 0) {
                    Execute(_buffer);
                    for (int i = 0; i < _buffer.Count; i++) {
                        _buffer[i].Release(this);
                    }
                    _buffer.Clear();
                }
            }
        }

        public override string ToString() {
            if(_toStringCache == null) {
                _toStringCache = "ReactiveSystem(" + GetType().Name + ")";
            }

            return _toStringCache;
        }

        ~ReactiveSystem () {
            Deactivate();
        }
    }
}
