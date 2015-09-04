using System;
using System.Collections.Generic;

namespace Entitas {
    public class Systems : IInitializeSystem, IExecuteSystem {

        protected readonly List<IInitializeSystem> _initializeSystems;
        protected readonly List<IExecuteSystem> _executeSystems;

        public Systems() {
            _initializeSystems = new List<IInitializeSystem>();
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
            var initializeSystem = reactiveSystem != null
                ? reactiveSystem.subsystem as IInitializeSystem
                : system as IInitializeSystem;

            if (initializeSystem != null) {
                _initializeSystems.Add(initializeSystem);
            }

            var executeSystem = system as IExecuteSystem;
            if (executeSystem != null) {
                _executeSystems.Add(executeSystem);
            }

            return this;
        }

        public virtual void Initialize() {
            for (int i = 0, initializeSysCount = _initializeSystems.Count; i < initializeSysCount; i++) {
                _initializeSystems[i].Initialize();
            }
        }

        public virtual void Execute() {
            for (int i = 0, exeSysCount = _executeSystems.Count; i < exeSysCount; i++) {
                _executeSystems[i].Execute();
            }
        }

        public virtual void ClearReactiveSystems() {
            for (int i = 0, exeSysCount = _executeSystems.Count; i < exeSysCount; i++) {
                var reactiveSystem = _executeSystems[i] as ReactiveSystem;
                if (reactiveSystem != null) {
                    reactiveSystem.Clear();
                }
                
                var nestedSystems = _executeSystems[i] as Systems;
                if (nestedSystems != null) {
                    nestedSystems.ClearReactiveSystems();
                }
            }
        }
    }
}