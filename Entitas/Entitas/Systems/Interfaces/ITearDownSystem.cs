namespace Entitas {

    /// <summary>
    /// Implement this interface if you want to create a system which should
    /// tear down once in the end.
    /// </summary>
    public interface ITearDownSystem : ISystem {

        void TearDown();
    }
}
