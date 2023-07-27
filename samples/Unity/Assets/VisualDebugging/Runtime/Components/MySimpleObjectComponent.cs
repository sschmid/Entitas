using System.Collections.Generic;
using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MySimpleObjectComponent : IComponent
{
    public MySimpleObject Value;
}

public class MySimpleObject
{
    public string Name;
    public int Age;
    public Dictionary<string, string> Data;
    public MyCustomObject MyCustomObject;
    public MySimpleObject Next;
}
