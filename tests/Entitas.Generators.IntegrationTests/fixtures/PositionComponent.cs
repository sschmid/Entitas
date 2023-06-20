using Entitas;
using Entitas.Generators.Attributes;

// ReSharper disable once CheckNamespace
namespace MyApp
{
    [Context("MyApp.MainContext")]
    partial class PositionComponent : IComponent
    {
        public int X;
        public int Y;
    }
}
