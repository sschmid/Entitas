using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyArrayComponent : IComponent
{
    public string[] Value;
}
