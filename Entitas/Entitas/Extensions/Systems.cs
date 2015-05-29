using System;
using System.Collections.Generic;

namespace Entitas {
    public class Systems : IStartSystem, IExecuteSystem {

        public ISystem[] systems { 
            get {
                if (_systemsCache == null) {
                    _systemsCache = _systems.ToArray();
                }

                return _systemsCache;
            }
        }

        public int startSystemsCount { get { return _startSystems.Count; } }
        public int executeSystemsCount { get { return _executeSystems.Count; } }

        protected readonly List<ISystem> _systems;
        protected ISystem[] _systemsCache;
        protected readonly List<IStartSystem> _startSystems;
        protected readonly List<IExecuteSystem> _executeSystems;

        public Systems() {
            _systems = new List<ISystem>();
            _startSystems = new List<IStartSystem>();
            _executeSystems = new List<IExecuteSystem>();
        }

        public virtual Systems Add<T>() {
            return Add(typeof(T));
        }

        public virtual Systems Add(Type systemType) {
            return Add((ISystem)Activator.CreateInstance(systemType));
        }

        public virtual Systems Add(ISystem system) {
            _systems.Add(system);
            _systemsCache = null;

            var reactiveSystem = system as ReactiveSystem;
            var startSystem = reactiveSystem != null
                ? reactiveSystem.subsystem as IStartSystem
                : system as IStartSystem;

            if (startSystem != null) {
                _startSystems.Add(startSystem);
            }

            var executeSystem = system as IExecuteSystem;
            if (executeSystem != null) {
                _executeSystems.Add(executeSystem);
            }

            return this;
        }

        public virtual void Start() {
            for (int i = 0, _startSystemsCount = _startSystems.Count; i < _startSystemsCount; i++) {
                _startSystems[i].Start();
            }
        }

        public virtual void Execute() {
            for (int i = 0, _executeSystemsCount = _executeSystems.Count; i < _executeSystemsCount; i++) {
                _executeSystems[i].Execute();
            }
        }
    }
}