using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyBoolComponent : IComponent
{
    public bool Value;
}
