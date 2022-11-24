using Entitas;
using Entitas.Plugins.Attributes;

public class PrimaryEntityIndexComponent : IComponent
{
    [PrimaryEntityIndex]
    public string Value;
}
