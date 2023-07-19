using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext)), Cleanup(CleanupMode.DestroyEntity)]
    public sealed class CleanupDestroyEntityNamespacedComponent : IComponent { }
}
