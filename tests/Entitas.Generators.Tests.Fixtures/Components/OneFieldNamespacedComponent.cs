using Entitas;

#pragma warning disable CS0649

namespace MyFeature
{
    [MyApp.Main.Context]
    partial class OneFieldNamespacedComponent : IComponent
    {
        public string Value;
    }
}
