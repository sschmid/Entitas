using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext)), Context(typeof(OtherContext))]
    [Unique]
    public sealed class LoadingComponent : IComponent { }
}
