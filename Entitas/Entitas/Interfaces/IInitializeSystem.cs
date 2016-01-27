namespace Entitas {

    /// Implement this interface if you want to create a system which should be initialized once in the beginning.
    public interface IInitializeSystem : ISystem {
        void Initialize();
    }
}

