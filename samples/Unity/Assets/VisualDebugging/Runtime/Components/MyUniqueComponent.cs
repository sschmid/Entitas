using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext)), Unique]
public sealed class MyUniqueComponent : IComponent
{
    public string Value;
}
