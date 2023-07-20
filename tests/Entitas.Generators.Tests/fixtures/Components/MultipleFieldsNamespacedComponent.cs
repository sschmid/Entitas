#nullable disable

using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext))]
    public sealed class MultipleFieldsNamespacedComponent : IComponent
    {
        public string Value1;
        public string Value2;
        public string Value3;
    }
}
