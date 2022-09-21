using System.Collections.Generic;

namespace Entitas
{
    /// Systems provide a convenient way to group systems.
    /// You can add IInitializeSystem, IExecuteSystem, ICleanupSystem,
    /// ITearDownSystem, ReactiveSystem and other nested Systems instances.
    /// All systems will be initialized and executed based on the order
    /// you added them.
    public class Systems : IInitializeSystem, IExecuteSystem, ICleanupSystem, ITearDownSystem
    {
        protected readonly List<IInitializeSystem> _initializeSystems;
        protected readonly List<IExecuteSystem> _executeSystems;
        protected readonly List<ICleanupSystem> _cleanupSystems;
        protected readonly List<ITearDownSystem> _tearDownSystems;

        /// Creates a new Systems instance.
        public Systems()
        {
            _initializeSystems = new List<IInitializeSystem>();
            _executeSystems = new List<IExecuteSystem>();
            _cleanupSystems = new List<ICleanupSystem>();
            _tearDownSystems = new List<ITearDownSystem>();
        }

        /// Adds the system instance to the systems list.
        public virtual Systems Add(ISystem system)
        {
            if (system is IInitializeSystem initializeSystem) _initializeSystems.Add(initializeSystem);
            if (system is IExecuteSystem executeSystem) _executeSystems.Add(executeSystem);
            if (system is ICleanupSystem cleanupSystem) _cleanupSystems.Add(cleanupSystem);
            if (system is ITearDownSystem tearDownSystem) _tearDownSystems.Add(tearDownSystem);

            return this;
        }

        /// Removes the system instance from the systems list.
        public void Remove(ISystem system)
        {
            if (system is IInitializeSystem initializeSystem) _initializeSystems.Remove(initializeSystem);
            if (system is IExecuteSystem executeSystem) _executeSystems.Remove(executeSystem);
            if (system is ICleanupSystem cleanupSystem) _cleanupSystems.Remove(cleanupSystem);
            if (system is ITearDownSystem tearDownSystem) _tearDownSystems.Remove(tearDownSystem);
        }

        /// Calls Initialize() on all IInitializeSystem and other
        /// nested Systems instances in the order you added them.
        public virtual void Initialize()
        {
            for (var i = 0; i < _initializeSystems.Count; i++)
                _initializeSystems[i].Initialize();
        }

        /// Calls Execute() on all IExecuteSystem and other
        /// nested Systems instances in the order you added them.
        public virtual void Execute()
        {
            for (var i = 0; i < _executeSystems.Count; i++)
                _executeSystems[i].Execute();
        }

        /// Calls Cleanup() on all ICleanupSystem and other
        /// nested Systems instances in the order you added them.
        public virtual void Cleanup()
        {
            for (var i = 0; i < _cleanupSystems.Count; i++)
                _cleanupSystems[i].Cleanup();
        }

        /// Calls TearDown() on all ITearDownSystem  and other
        /// nested Systems instances in the order you added them.
        public virtual void TearDown()
        {
            for (var i = 0; i < _tearDownSystems.Count; i++)
                _tearDownSystems[i].TearDown();
        }

        /// Activates all ReactiveSystems in the systems list.
        public void ActivateReactiveSystems()
        {
            for (var i = 0; i < _executeSystems.Count; i++)
            {
                var system = _executeSystems[i];
                if (system is IReactiveSystem reactiveSystem)
                    reactiveSystem.Activate();

                if (system is Systems nestedSystems)
                    nestedSystems.ActivateReactiveSystems();
            }
        }

        /// Deactivates all ReactiveSystems in the systems list.
        /// This will also clear all ReactiveSystems.
        /// This is useful when you want to soft-restart your application and
        /// want to reuse your existing system instances.
        public void DeactivateReactiveSystems()
        {
            for (var i = 0; i < _executeSystems.Count; i++)
            {
                var system = _executeSystems[i];
                if (system is IReactiveSystem reactiveSystem)
                    reactiveSystem.Deactivate();

                if (system is Systems nestedSystems)
                    nestedSystems.DeactivateReactiveSystems();
            }
        }

        /// Clears all ReactiveSystems in the systems list.
        public void ClearReactiveSystems()
        {
            for (var i = 0; i < _executeSystems.Count; i++)
            {
                var system = _executeSystems[i];
                if (system is IReactiveSystem reactiveSystem)
                    reactiveSystem.Clear();

                if (system is Systems nestedSystems)
                    nestedSystems.ClearReactiveSystems();
            }
        }
    }
}
