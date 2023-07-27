using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyFlagsComponent : IComponent
{
    public MyFlags Value;
}

[System.Flags]
public enum MyFlags
{
    Item1 = 1,
    Item2 = 2,
    Item3 = 4,
    Item4 = 8
}
