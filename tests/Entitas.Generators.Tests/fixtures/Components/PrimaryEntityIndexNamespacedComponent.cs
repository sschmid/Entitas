#nullable disable

using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext))]
    public sealed class PrimaryEntityIndexNamespacedComponent : IComponent
    {
        [EntityIndex(true)]
        public string Value;
    }
}
