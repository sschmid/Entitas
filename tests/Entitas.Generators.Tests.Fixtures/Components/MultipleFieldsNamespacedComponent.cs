using Entitas;

namespace MyFeature
{
    [MyApp.Main.Context]
    partial class MultipleFieldsNamespacedComponent : IComponent
    {
        public string Value1;
        public string Value2;
        public string Value3;
    }
}
