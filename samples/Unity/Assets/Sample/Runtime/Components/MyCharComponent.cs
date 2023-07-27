using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyCharComponent : IComponent
{
    public char Value;
}
