namespace Entitas {

    /// <summary>
    /// Implement this interface if you want to create a system which should
    /// execute cleanup logic after execution.
    /// </summary>
    public interface ICleanupSystem : ISystem {

        void Cleanup();
    }
}
