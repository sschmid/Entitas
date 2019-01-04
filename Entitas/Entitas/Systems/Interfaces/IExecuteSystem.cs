namespace Entitas {

    /// <summary>
    /// Implement this interface if you want to create a system which should be
    /// executed every frame.
    /// </summary>
    public interface IExecuteSystem : ISystem {

        void Execute();
    }
}
