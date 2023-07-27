using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyIntComponent : IComponent
{
    public int Value;
}
