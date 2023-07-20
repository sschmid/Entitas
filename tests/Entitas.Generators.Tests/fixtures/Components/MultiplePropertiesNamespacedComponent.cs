#nullable disable

using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext))]
    public sealed class MultiplePropertiesNamespacedComponent : IComponent
    {
        public string Value1 { get; set; }
        public string Value2 { get; set; }

        public string Value3
        {
            get => _value3;
            set => _value3 = value;
        }

        string _value3;
    }
}
