using System;
using System.Collections.Generic;

namespace Entitas {

    /// A ReactiveSystem manages your implementation of a
    /// IReactiveSystem subsystem.
    /// It will only call subsystem.Execute() if there were changes based on
    /// the triggers and eventTypes specified by your subsystem
    /// and will only pass in changed entities. A common use-case is to react
    /// to changes, e.g. a change of the position of an entity to update the
    /// gameObject.transform.position of the related gameObject.
    public class ReactiveSystem : IExecuteSystem {

        /// Returns the subsystem which will be managed by this
        /// instance of ReactiveSystem.
        public IReactiveSystem subsystem { get { return _subsystem; } }

        readonly IReactiveSystem _subsystem;
        readonly EntityCollector _collector;
        readonly Func<Entity, bool> _filter;
        readonly bool _clearAfterExecute;
        readonly List<Entity> _buffer;
        string _toStringCache;

        public ReactiveSystem(IReactiveSystem subSystem) :
            this(subSystem, Pools.sharedInstance) {
        }

        public ReactiveSystem(IReactiveSystem subSystem, Pools pools) :
            this(subSystem, subSystem.GetTrigger(pools)) {
        }

        ReactiveSystem(IReactiveSystem subSystem, EntityCollector collector) {
            _subsystem = subSystem;

            var filterEntities = subSystem as IFilterEntities;
            if(filterEntities != null) {
                _filter = filterEntities.filter;
            }

            _clearAfterExecute = (subSystem as IClearReactiveSystem) != null;

            _collector = collector;
            _buffer = new List<Entity>();
        }

        /// Activates the ReactiveSystem and starts observing changes
        /// based on the triggers and eventTypes specified by the subsystem.
        /// ReactiveSystem are activated by default.
        public void Activate() {
            _collector.Activate();
        }

        /// Deactivates the ReactiveSystem.
        /// No changes will be tracked while deactivated.
        /// This will also clear the ReactiveSystems.
        /// ReactiveSystem are activated by default
        public void Deactivate() {
            _collector.Deactivate();
        }

        /// Clears all accumulated changes.
        public void Clear() {
            _collector.ClearCollectedEntities();
        }

        /// Will call subsystem.Execute() with changed entities
        /// if there are any. Otherwise it will not call subsystem.Execute().
        public void Execute() {
            if(_collector.collectedEntities.Count != 0) {
                foreach(var e in _collector.collectedEntities) {
                    if(_filter == null || _filter(e)) {
                        _buffer.Add(e.Retain(this));
                    }
                }

                _collector.ClearCollectedEntities();
                if(_buffer.Count != 0) {
                    _subsystem.Execute(_buffer);
                    for (int i = 0; i < _buffer.Count; i++) {
                        _buffer[i].Release(this);
                    }
                    _buffer.Clear();
                    if(_clearAfterExecute) {
                        _collector.ClearCollectedEntities();
                    }
                }
            }
        }

        public override string ToString() {
            if(_toStringCache == null) {
                _toStringCache = "ReactiveSystem(" + subsystem + ")";
            }

            return _toStringCache;
        }

        ~ReactiveSystem () {
            Deactivate();
        }
    }
}
