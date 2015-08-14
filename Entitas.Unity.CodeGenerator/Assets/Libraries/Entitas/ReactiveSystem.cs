using System;
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
            this(pool, subSystem, new [] { subSystem.trigger }, new [] { subSystem.eventType }) {
        }

        public ReactiveSystem(Pool pool, IMultiReactiveSystem subSystem) :
            this(pool, subSystem, subSystem.triggers, subSystem.eventTypes) {
        }

        ReactiveSystem(Pool pool, IReactiveExecuteSystem subSystem, IMatcher[] triggers, GroupEventType[] eventTypes) {
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
            for (int i = 0, triggersLength = triggers.Length; i < triggersLength; i++) {
                groups[i] = pool.GetGroup(triggers[i]);
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
                                _buffer.Add(e);
                            }
                        }
                    } else {
                        foreach (var e in _observer.collectedEntities) {
                            if (_ensureComponents.Matches(e)) {
                                _buffer.Add(e);
                            }
                        }
                    }
                } else if (_excludeComponents != null) {
                    foreach (var e in _observer.collectedEntities) {
                        if (!_excludeComponents.Matches(e)) {
                            _buffer.Add(e);
                        }
                    }
                } else {
                    _buffer.AddRange(_observer.collectedEntities);
                }

                _observer.ClearCollectedEntities();
                if (_buffer.Count != 0) {
                    _subsystem.Execute(_buffer);
                    _buffer.Clear();
                }
            }
        }
    }
}

