using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext)), Unique]
    public sealed class UniqueNamespacedComponent : IComponent { }
}
