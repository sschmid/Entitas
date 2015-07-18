using System;
using System.Collections.Generic;

namespace Entitas {
    public class Systems : IStartSystem, IExecuteSystem {

        protected readonly List<IStartSystem> _startSystems;
        protected readonly List<IExecuteSystem> _executeSystems;

        public Systems() {
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
            for (int i = 0, startSysCount = _startSystems.Count; i < startSysCount; i++) {
                _startSystems[i].Start();
            }
        }

        public virtual void Execute() {
            for (int i = 0, exeSysCount = _executeSystems.Count; i < exeSysCount; i++) {
                _executeSystems[i].Execute();
            }
        }
    }
}