using System.Collections.Generic;

namespace Entitas {

    /// Systems provide a convenient way to group systems. You can add IInitializeSystem, IExecuteSystem, ICleanupSystem, IDeinitializeSystem, ReactiveSystem and other nested Systems instances.
    /// All systems will be initialized and executed based on the order you added them.
    public class Systems : IInitializeSystem, IExecuteSystem, ICleanupSystem, IDeinitializeSystem {

        protected readonly List<IInitializeSystem> _initializeSystems;
        protected readonly List<IExecuteSystem> _executeSystems;
        protected readonly List<ICleanupSystem> _cleanupSystems;
        protected readonly List<IDeinitializeSystem> _deinitializeSystems;

        /// Creates a new Systems instance.
        public Systems() {
            _initializeSystems = new List<IInitializeSystem>();
            _executeSystems = new List<IExecuteSystem>();
            _cleanupSystems = new List<ICleanupSystem>();
            _deinitializeSystems = new List<IDeinitializeSystem>();
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

            var cleanupSystem = reactiveSystem != null
                ? reactiveSystem.subsystem as ICleanupSystem
                : system as ICleanupSystem;

            if (cleanupSystem != null) {
                _cleanupSystems.Add(cleanupSystem);
            }

            var deinitializeSystem = reactiveSystem != null
                ? reactiveSystem.subsystem as IDeinitializeSystem
                : system as IDeinitializeSystem;

            if (deinitializeSystem != null) {
                _deinitializeSystems.Add(deinitializeSystem);
            }

            return this;
        }

        /// Calls Initialize() on all IInitializeSystem, ReactiveSystem and other nested Systems instances in the order you added them.
        public virtual void Initialize() {
            for (int i = 0; i < _initializeSystems.Count; i++) {
                _initializeSystems[i].Initialize();
            }
        }

        /// Calls Execute() on all IExecuteSystem, ReactiveSystem and other nested Systems instances in the order you added them.
        public virtual void Execute() {
            for (int i = 0; i < _executeSystems.Count; i++) {
                _executeSystems[i].Execute();
            }
        }

        /// Calls Cleanup() on all ICleanupSystem, ReactiveSystem and other nested Systems instances in the order you added them.
        public virtual void Cleanup() {
            for (int i = 0; i < _cleanupSystems.Count; i++) {
                _cleanupSystems[i].Cleanup();
            }
        }

        /// Calls Deinitialize() on all IDeinitializeSystem, ReactiveSystem and other nested Systems instances in the order you added them.
        public virtual void Deinitialize() {
            for (int i = 0; i < _deinitializeSystems.Count; i++) {
                _deinitializeSystems[i].Deinitialize();
            }
        }

        /// Activates all ReactiveSystems in the systems list.
        public virtual void ActivateReactiveSystems() {
            for (int i = 0; i < _executeSystems.Count; i++) {
                var system = _executeSystems[i];
                var reactiveSystem = system as ReactiveSystem;
                if (reactiveSystem != null) {
                    reactiveSystem.Activate();
                }

                var nestedSystems = system as Systems;
                if (nestedSystems != null) {
                    nestedSystems.ActivateReactiveSystems();
                }
            }
        }

        /// Deactivates all ReactiveSystems in the systems list. This will also clear all ReactiveSystems.
        /// This is useful when you want to soft-restart your application and want to reuse your existing system instances.
        public virtual void DeactivateReactiveSystems() {
            for (int i = 0; i < _executeSystems.Count; i++) {
                var system = _executeSystems[i];
                var reactiveSystem = system as ReactiveSystem;
                if (reactiveSystem != null) {
                    reactiveSystem.Deactivate();
                }

                var nestedSystems = system as Systems;
                if (nestedSystems != null) {
                    nestedSystems.DeactivateReactiveSystems();
                }
            }
        }

        /// Clears all ReactiveSystems in the systems list.
        public virtual void ClearReactiveSystems() {
            for (int i = 0; i < _executeSystems.Count; i++) {
                var system = _executeSystems[i];
                var reactiveSystem = system as ReactiveSystem;
                if (reactiveSystem != null) {
                    reactiveSystem.Clear();
                }
                
                var nestedSystems = system as Systems;
                if (nestedSystems != null) {
                    nestedSystems.ClearReactiveSystems();
                }
            }
        }
    }
}