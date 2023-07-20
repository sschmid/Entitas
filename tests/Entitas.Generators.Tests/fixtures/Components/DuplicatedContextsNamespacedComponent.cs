#nullable disable

using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext))]
    [Context(typeof(MainContext))]
    public sealed class DuplicatedContextsNamespacedComponent : IComponent { }
}
