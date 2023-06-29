using Entitas;

namespace MyFeature
{
    [MyApp.Main.Context, Other.ContextAttribute]
    public sealed class PositionComponent : IComponent
    {
        public int X;
        public int Y;
    }
}
