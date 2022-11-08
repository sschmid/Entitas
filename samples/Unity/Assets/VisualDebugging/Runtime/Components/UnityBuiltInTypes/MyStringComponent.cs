using Entitas;

[Game, Input]
public class MyStringComponent : IComponent
{
    public string Value;

    public override string ToString() => $"MyString({Value})";
}
