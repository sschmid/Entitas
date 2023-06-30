using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [MyApp.Main.Context]
    [Context(typeof(MainContext))]
    public sealed class SomeNamespacedComponent : IComponent { }
}
