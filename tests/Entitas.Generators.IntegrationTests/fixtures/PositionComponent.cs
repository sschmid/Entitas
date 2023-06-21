using Entitas;

namespace MyFeature
{
    [MyApp.Main.Context, Other.Context]
    public sealed class PositionComponent : IComponent
    {
        public int X;
        public int Y;
    }
}
