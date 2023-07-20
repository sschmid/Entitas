#nullable disable

using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext)), Unique]
    public sealed class UniqueOneFieldNamespacedComponent : IComponent
    {
        public string Value;
    }
}
