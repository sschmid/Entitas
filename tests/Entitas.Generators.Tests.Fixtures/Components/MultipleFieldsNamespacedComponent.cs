using Entitas;

#pragma warning disable CS0649

namespace MyFeature
{
    [MyApp.Main.Context]
    public sealed class MultipleFieldsNamespacedComponent : IComponent
    {
        public string Value1;
        public string Value2;
        public string Value3;
    }
}
