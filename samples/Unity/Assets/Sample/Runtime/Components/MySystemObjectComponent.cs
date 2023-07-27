using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MySystemObjectComponent : IComponent
{
    public System.Object Value;
}
