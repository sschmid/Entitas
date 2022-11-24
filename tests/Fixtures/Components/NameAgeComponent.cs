using Entitas;
using Entitas.Plugins.Attributes;

[Context("Test1"), Context("Test2")]
public sealed class NameAgeComponent : IComponent
{
    public string Name;
    public int Age;

    public override string ToString()
    {
        return $"NameAge({Name}, {Age})";
    }
}
