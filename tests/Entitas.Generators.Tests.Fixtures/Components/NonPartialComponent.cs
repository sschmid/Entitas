using Entitas;

namespace MyFeature
{
    [MyApp.Main.Context]
    public sealed class NonPartialComponent : IComponent
    {
        public string Value;
    }
}
