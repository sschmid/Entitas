using System.Collections.Generic;

namespace Entitas {
    public class ReactiveSystem : IExecuteSystem {
        public IReactiveExecuteSystem subsystem { get { return _subsystem; } }

        readonly IReactiveExecuteSystem _subsystem;
        readonly GroupObserver _observer;
        readonly IMatcher _ensureComponents;
        readonly IMatcher _excludeComponents;
        readonly bool _clearAfterExecute;
        readonly List<Entity> _buffer;

        public ReactiveSystem(Pool pool, IReactiveSystem subSystem) :
            this(pool, subSystem, new [] { subSystem.trigger }) {
        }

        public ReactiveSystem(Pool pool, IMultiReactiveSystem subSystem) :
            this(pool, subSystem, subSystem.triggers) {
        }

        ReactiveSystem(Pool pool, IReactiveExecuteSystem subSystem, TriggerOnEvent[] triggers) {
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

            var triggersLength = triggers.Length;
            var groups = new Group[triggersLength];
            var eventTypes = new GroupEventType[triggersLength];
            for (int i = 0; i < triggersLength; i++) {
                var trigger = triggers[i];
                groups[i] = pool.GetGroup(trigger.trigger);
                eventTypes[i] = trigger.eventType;
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

        public void Clear() {
            _observer.ClearCollectedEntities();
        }

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
    }
}

