using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyJaggedArrayComponent : IComponent
{
    public string[][] Value;
}
