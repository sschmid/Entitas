using Entitas;
using Entitas.Plugins.Attributes;

public class MultiplePrimaryEntityIndicesComponent : IComponent
{
    [PrimaryEntityIndex]
    public string value;

    [PrimaryEntityIndex]
    public string value2;
}
