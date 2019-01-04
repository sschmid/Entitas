namespace Entitas {

    /// <summary>
    /// Implement this interface if you want to create a system which should be
    /// initialized once in the beginning.
    /// </summary>
    public interface IInitializeSystem : ISystem {

        void Initialize();
    }
}
