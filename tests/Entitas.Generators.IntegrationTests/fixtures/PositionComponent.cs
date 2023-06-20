using Entitas;

namespace MyFeature
{
    [MyApp.Main.Context, Other.Context]
    partial class PositionComponent : IComponent
    {
        public int X;
        public int Y;
    }
}
