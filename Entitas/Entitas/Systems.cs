using System;
using System.Collections.Generic;

namespace Entitas {
    public class Systems : IStartSystem, IExecuteSystem {

        protected readonly List<IStartSystem> _startSystems;
        protected readonly List<IExecuteSystem> _executeSystems;
        protected readonly List<Pool> _pools;

        public Systems() {
            _startSystems = new List<IStartSystem>();
            _executeSystems = new List<IExecuteSystem>();
            _pools = new List<Pool>();
        }

        public virtual Systems Add<T>() {
            return Add(typeof(T));
        }

        public virtual Systems Add(Type systemType) {
            return Add((ISystem)Activator.CreateInstance(systemType));
        }

        public virtual Systems Add(ISystem system) {
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

        public virtual Systems Add(Pool pool) {
            _pools.Add(pool);

            return this;
        }

        public virtual void Start() {
            for (int i = 0, startSysCount = _startSystems.Count; i < startSysCount; i++) {
                _startSystems[i].Start();
            }
        }

        public virtual void Execute() {
            for (int i = 0, exeSysCount = _executeSystems.Count; i < exeSysCount; i++) {
                _executeSystems[i].Execute();
            }

            for (int i = 0, poolsCount = _pools.Count; i < poolsCount; i++) {
                _pools[i].EndLoop();

                foreach (var e in _pools[i].GetEntities()) {
                    if(e.IsDestroyed()) _pools[i].DestroyEntity(e);
                }
            }
        }
    }
}