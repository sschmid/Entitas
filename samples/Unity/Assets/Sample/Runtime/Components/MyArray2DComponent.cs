using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyArray2DComponent : IComponent
{
    public string[,] Value;
}
