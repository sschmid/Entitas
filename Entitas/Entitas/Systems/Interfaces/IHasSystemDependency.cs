namespace Entitas
{
    /// Implement this interface to have dependencies to another systems
    /// that live in the same system parent
    /// The [Systems] class is resolving the dependencies
    public interface IHasSystemDependency<T> : ISystem where T : ISystem {
        void SetSystem(T dependency);
    }
}