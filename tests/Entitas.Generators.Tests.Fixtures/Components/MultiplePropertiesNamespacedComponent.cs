using Entitas;

namespace MyFeature
{
    [MyApp.Main.Context]
    public sealed class MultiplePropertiesNamespacedComponent : IComponent
    {
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
    }
}
