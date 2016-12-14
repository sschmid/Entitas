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
        readonly IMatcher _ensureComponents;
        readonly IMatcher _excludeComponents;
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
            var ensureComponents = subSystem as IEnsureComponents;
            if(ensureComponents != null) {
                _ensureComponents = ensureComponents.ensureComponents;
            }
            var excludeComponents = subSystem as IExcludeComponents;
            if(excludeComponents != null) {
                _excludeComponents = excludeComponents.excludeComponents;
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
                if(_ensureComponents != null) {
                    if(_excludeComponents != null) {
                        foreach(var e in _collector.collectedEntities) {
                            if(_ensureComponents.Matches(e) &&
                               !_excludeComponents.Matches(e)) {
                                _buffer.Add(e.Retain(this));
                            }
                        }
                    } else {
                        foreach(var e in _collector.collectedEntities) {
                            if(_ensureComponents.Matches(e)) {
                                _buffer.Add(e.Retain(this));
                            }
                        }
                    }
                } else if(_excludeComponents != null) {
                    foreach(var e in _collector.collectedEntities) {
                        if(!_excludeComponents.Matches(e)) {
                            _buffer.Add(e.Retain(this));
                        }
                    }
                } else {
                    foreach(var e in _collector.collectedEntities) {
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
