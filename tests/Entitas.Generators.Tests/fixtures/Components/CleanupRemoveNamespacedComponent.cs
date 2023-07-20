#nullable disable

using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext)), Cleanup(CleanupMode.RemoveComponent)]
    public sealed class CleanupRemoveNamespacedComponent : IComponent { }
}
