using System.Collections.Generic;

namespace Entitas {
    public class ReactiveSystem : IExecuteSystem {
        public IReactiveExecuteSystem subsystem { get { return _subsystem; } }

        readonly IReactiveExecuteSystem _subsystem;
        readonly GroupObserver _observer;
        readonly IMatcher _ensureComponents;
        readonly IMatcher _excludeComponents;
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

            var groups = new Group[triggers.Length];
            var eventTypes = new GroupEventType[triggers.Length];
            for (int i = 0, triggersLength = triggers.Length; i < triggersLength; i++) {
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

        public void Execute() {
            if (_observer.collectedEntities.Count != 0) {
                if (_ensureComponents != null) {
                    if (_excludeComponents != null) {
                        foreach (var e in _observer.collectedEntities) {
                            if (_ensureComponents.Matches(e) && !_excludeComponents.Matches(e)) {
                                e.Retain();
                                _buffer.Add(e);
                            }
                        }
                    } else {
                        foreach (var e in _observer.collectedEntities) {
                            if (_ensureComponents.Matches(e)) {
                                e.Retain();
                                _buffer.Add(e);
                            }
                        }
                    }
                } else if (_excludeComponents != null) {
                    foreach (var e in _observer.collectedEntities) {
                        if (!_excludeComponents.Matches(e)) {
                            e.Retain();
                            _buffer.Add(e);
                        }
                    }
                } else {
                    foreach (var e in _observer.collectedEntities) {
                        e.Retain();
                        _buffer.Add(e);
                    }
                }

                _observer.ClearCollectedEntities();
                if (_buffer.Count != 0) {
                    _subsystem.Execute(_buffer);
                    for (int i = 0, bufferCount = _buffer.Count; i < bufferCount; i++) {
                        _buffer[i].Release();
                    }
                    _buffer.Clear();
                }
            }
        }
    }
}

