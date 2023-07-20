using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext))]
    public sealed class SomeNamespacedComponent : IComponent { }
}
