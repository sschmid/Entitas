using System.Collections.Generic;
using Entitas;

[Game]
public class MySimpleObjectComponent : IComponent
{
    public MySimpleObject Value;
}

public class MySimpleObject
{
    public string Name;
    public int Age;
    public Dictionary<string, string> Data;
    public MyCustomObject MyCustomObject;
    public MyIntVector2 MyIntVector;
    public MySimpleObject Next;
}
