namespace Entitas {

    /// This is the base interface for all systems.
    /// It's not meant to be implemented.
    /// Use IInitializeSystem, IExecuteSystem,
    /// ICleanupSystem or ITearDownSystem.
    public interface ISystem {
    }

    /// Implement this interface if you want to create a system which should be
    /// initialized once in the beginning.
    public interface IInitializeSystem : ISystem {
        void Initialize();
    }

    /// Implement this interface if you want to create a system which should be
    /// executed every frame.
    public interface IExecuteSystem : ISystem {
        void Execute();
    }

    /// Implement this interface if you want to create a system which should
    /// execute cleanup logic after execution.
    public interface ICleanupSystem : ISystem {
        void Cleanup();
    }

    /// Implement this interface if you want to create a system which should
    /// tear down once in the end.
    public interface ITearDownSystem : ISystem {
        void TearDown();
    }
}
