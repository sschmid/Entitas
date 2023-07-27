using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyUnsupportedObjectComponent : IComponent
{
    public UnsupportedObject Value;
}

public class UnsupportedObject
{
    public string Value;

    public UnsupportedObject(string value)
    {
        Value = value;
    }
}
