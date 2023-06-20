using Entitas;
using Entitas.Generators.Attributes;

namespace MyFeature
{
    [Context("MyApp.MainContext")]
    partial class PositionComponent : IComponent
    {
        public int X;
        public int Y;
    }
}
