using Entitas;

#pragma warning disable CS0649

namespace MyFeature
{
    [MyApp.Main.Context]
    public sealed class OneFieldNamespacedComponent : IComponent
    {
        public string Value;
    }
}
