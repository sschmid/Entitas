using System.Collections.Generic;

namespace Entitas {

    /// A ReactiveSystem manages your implementation of a IReactiveSystem or a IMultiReactiveSystem subsystem.
    /// It will only call subsystem.Execute() if there were changes based on the triggers and eventTypes specified by your subsystem
    /// and will only pass in changed entities. A common use-case is to react to changes,
    /// e.g. a change of the position of an entity to update the gameObject.transform.position of the related gameObject.
    /// Recommended way to create systems in general: pool.CreateSystem<RenderPositionSystem>();
    public class ReactiveSystem : IExecuteSystem {

        /// Returns the subsystem which will be managed my this instance of ReactiveSystem.
        public IReactiveExecuteSystem subsystem { get { return _subsystem; } }

        readonly IReactiveExecuteSystem _subsystem;
        readonly GroupObserver _observer;
        readonly IMatcher _ensureComponents;
        readonly IMatcher _excludeComponents;
        readonly bool _clearAfterExecute;
        readonly List<Entity> _buffer;
        string _toStringCache;

        /// Recommended way to create systems in general: pool.CreateSystem<RenderPositionSystem>();
        public ReactiveSystem(Pool pool, IReactiveSystem subSystem) :
            this(subSystem, createGroupObserver(pool, new [] { subSystem.trigger })) {
        }

        /// Recommended way to create systems in general: pool.CreateSystem<RenderPositionSystem>();
        public ReactiveSystem(Pool pool, IMultiReactiveSystem subSystem) :
            this(subSystem, createGroupObserver(pool, subSystem.triggers)) {
        }

        /// Recommended way to create systems in general: pool.CreateSystem<RenderPositionSystem>();
        public ReactiveSystem(IGroupObserverSystem subSystem) :
            this(subSystem, subSystem.groupObserver) {
        }

        ReactiveSystem(IReactiveExecuteSystem subSystem, GroupObserver groupObserver) {
            _subsystem = subSystem;
            var ensureComponents = subSystem as IEnsureComponents;
            if (ensureComponents != null) {
                _ensureComponents = ensureComponents.ensureComponents;
            }
            var excludeComponents = subSystem as IExcludeComponents;
            if (excludeComponents != null) {
                _excludeComponents = excludeComponents.excludeComponents;
            }

            _clearAfterExecute = (subSystem as IClearReactiveSystem) != null;

            _observer = groupObserver;
            _buffer = new List<Entity>();
        }

        static GroupObserver createGroupObserver(Pool pool, TriggerOnEvent[] triggers) {
            var triggersLength = triggers.Length;
            var groups = new Group[triggersLength];
            var eventTypes = new GroupEventType[triggersLength];
            for (int i = 0; i < triggersLength; i++) {
                var trigger = triggers[i];
                groups[i] = pool.GetGroup(trigger.trigger);
                eventTypes[i] = trigger.eventType;
            }

            return new GroupObserver(groups, eventTypes);
        }

        /// Activates the ReactiveSystem (ReactiveSystem are activated by default) and starts observing changes
        /// based on the triggers and eventTypes specified by the subsystem.
        public void Activate() {
            _observer.Activate();
        }

        /// Deactivates the ReactiveSystem (ReactiveSystem are activated by default).
        /// No changes will be tracked while deactivated.
        /// This will also clear the ReactiveSystems.
        public void Deactivate() {
            _observer.Deactivate();
        }

        /// Clears all accumulated changes.
        public void Clear() {
            _observer.ClearCollectedEntities();
        }

        /// Will call subsystem.Execute() with changed entities if there are any. Otherwise it will not call subsystem.Execute().
        public void Execute() {
            if (_observer.collectedEntities.Count != 0) {
                if (_ensureComponents != null) {
                    if (_excludeComponents != null) {
                        foreach (var e in _observer.collectedEntities) {
                            if (_ensureComponents.Matches(e) && !_excludeComponents.Matches(e)) {
                                _buffer.Add(e.Retain(this));
                            }
                        }
                    } else {
                        foreach (var e in _observer.collectedEntities) {
                            if (_ensureComponents.Matches(e)) {
                                _buffer.Add(e.Retain(this));
                            }
                        }
                    }
                } else if (_excludeComponents != null) {
                    foreach (var e in _observer.collectedEntities) {
                        if (!_excludeComponents.Matches(e)) {
                            _buffer.Add(e.Retain(this));
                        }
                    }
                } else {
                    foreach (var e in _observer.collectedEntities) {
                        _buffer.Add(e.Retain(this));
                    }
                }

                _observer.ClearCollectedEntities();
                if (_buffer.Count != 0) {
                    _subsystem.Execute(_buffer);
                    for (int i = 0, bufferCount = _buffer.Count; i < bufferCount; i++) {
                        _buffer[i].Release(this);
                    }
                    _buffer.Clear();
                    if (_clearAfterExecute) {
                        _observer.ClearCollectedEntities();
                    }
                }
            }
        }

        public override string ToString() {
            if (_toStringCache == null) {
                _toStringCache = "ReactiveSystem(" + subsystem + ")";
            }

            return _toStringCache;
        }

        ~ReactiveSystem () {
            Deactivate();
        }
    }
}

