namespace Entitas
{
    /// Implement this interface if you want to create a system which should
    /// execute cleanup logic after execution.
    public interface ICleanupSystem : ISystem
    {
        void Cleanup();
    }
}
