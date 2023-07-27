using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyEnumComponent : IComponent
{
    public MyEnum Value;
}

public enum MyEnum
{
    Item1,
    Item2,
    Item3
}
