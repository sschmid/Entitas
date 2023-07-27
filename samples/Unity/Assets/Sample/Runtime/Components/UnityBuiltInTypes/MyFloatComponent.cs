using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyFloatComponent : IComponent
{
    public float Value;
}
