using Entitas;

[Game]
public class MyCustomObjectComponent : IComponent
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
