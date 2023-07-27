using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyPersonComponent : IComponent
{
    public string Name;
    public string Gender;
}
