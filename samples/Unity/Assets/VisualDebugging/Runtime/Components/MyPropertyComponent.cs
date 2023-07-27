using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyPropertyComponent : IComponent
{
    public string Value { get; set; }
}
