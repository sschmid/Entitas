using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyDoubleComponent : IComponent
{
    public double Value;
}
