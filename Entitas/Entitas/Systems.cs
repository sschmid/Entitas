using System;
using System.Collections.Generic;

namespace Entitas {

    /// Systems provide a convenient way to group systems. You can add IInitializeSystem, IExecuteSystem, ReactiveSystem and other nested Systems instances.
    /// All systems will be initialized and executed based on the order you added them.
    public class Systems : IInitializeSystem, IExecuteSystem {

        protected readonly List<IInitializeSystem> _initializeSystems;
        protected readonly List<IExecuteSystem> _executeSystems;

        /// Creates a new Systems instance.
        public Systems() {
            _initializeSystems = new List<IInitializeSystem>();
            _executeSystems = new List<IExecuteSystem>();
        }

        /// Creates a new instance of the specified type and adds it to the systems list.
        public virtual Systems Add<T>() {
            return Add(typeof(T));
        }

        /// Creates a new instance of the specified type and adds it to the systems list.
        public virtual Systems Add(Type systemType) {
            return Add((ISystem)Activator.CreateInstance(systemType));
        }

        /// Adds the system instance to the systems list.
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

        /// Calls Initialize() on all IInitializeSystem in the order you added them.
        public virtual void Initialize() {
            for (int i = 0, initializeSysCount = _initializeSystems.Count; i < initializeSysCount; i++) {
                _initializeSystems[i].Initialize();
            }
        }

        /// Calls Execute() on all IExecuteSystem, ReactiveSystem and other nested Systems instances in the order you added them.
        public virtual void Execute() {
            for (int i = 0, exeSysCount = _executeSystems.Count; i < exeSysCount; i++) {
                _executeSystems[i].Execute();
            }
        }

        /// Activates all ReactiveSystems in the systems list.
        public virtual void ActivateReactiveSystems() {
            for (int i = 0, exeSysCount = _executeSystems.Count; i < exeSysCount; i++) {
                var reactiveSystem = _executeSystems[i] as ReactiveSystem;
                if (reactiveSystem != null) {
                    reactiveSystem.Activate();
                }

                var nestedSystems = _executeSystems[i] as Systems;
                if (nestedSystems != null) {
                    nestedSystems.ActivateReactiveSystems();
                }
            }
        }

        /// Deactivates all ReactiveSystems in the systems list. This will also clear all ReactiveSystems.
        /// This is useful when you want to soft-restart your application and want to reuse your existing system instances.
        public virtual void DeactivateReactiveSystems() {
            for (int i = 0, exeSysCount = _executeSystems.Count; i < exeSysCount; i++) {
                var reactiveSystem = _executeSystems[i] as ReactiveSystem;
                if (reactiveSystem != null) {
                    reactiveSystem.Deactivate();
                }

                var nestedSystems = _executeSystems[i] as Systems;
                if (nestedSystems != null) {
                    nestedSystems.DeactivateReactiveSystems();
                }
            }
        }

        /// Clears all ReactiveSystems in the systems list.
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