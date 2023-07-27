using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyCustomObjectComponent : IComponent
{
    public MyCustomObject Value;
}

public class MyCustomObject
{
    public string Name;

    public MyCustomObject(string name)
    {
        Name = name;
    }
}
