using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyArray3DComponent : IComponent
{
    public string[,,] Value;
}
