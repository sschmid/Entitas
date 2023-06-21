using Entitas;

#pragma warning disable CS0649

namespace MyFeature
{
    [MyApp.Main.Context]
    class NonPublicComponent : IComponent
    {
        public string Value;
    }
}
