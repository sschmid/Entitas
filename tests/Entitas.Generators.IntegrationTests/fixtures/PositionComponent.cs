using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext)), Context(typeof(OtherContext))]
    public sealed class PositionComponent : IComponent
    {
        public int X;
        public int Y;
    }
}
