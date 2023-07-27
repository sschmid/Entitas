using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext)), Context(typeof(InputContext))]
public sealed class MyStringComponent : IComponent
{
    public string Value;

    public override string ToString() => $"MyString({Value})";
}
