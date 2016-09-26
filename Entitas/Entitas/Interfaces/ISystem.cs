namespace Entitas {

    /// This is the base interface for all systems. It's not meant to be implemented.
    /// Use IInitializeSystem, IExecuteSystem, IReactiveSystem, IMultiReactiveSystem, IEntityCollectorSystem, ICleanupSystem or ITearDownSystem.
    public interface ISystem {
    }
}
