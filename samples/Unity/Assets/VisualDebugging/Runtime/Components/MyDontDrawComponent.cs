using Entitas;
using Entitas.Generators.Attributes;
using Entitas.Unity;

[Context(typeof(GameContext)), DontDrawComponent]
public sealed class MyDontDrawComponent : IComponent
{
    public MySimpleObject Value;
}
