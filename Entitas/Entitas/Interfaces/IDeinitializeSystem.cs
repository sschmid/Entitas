namespace Entitas {

    /// Implement this interface if you want to create a system which should be deinitialized once in the end.
    public interface IDeinitializeSystem : ISystem {
        void Deinitialize();
    }
}

