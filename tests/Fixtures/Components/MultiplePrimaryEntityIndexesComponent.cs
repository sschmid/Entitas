using Entitas;
using Entitas.Plugins.Attributes;

public class MultiplePrimaryEntityIndexesComponent : IComponent
{
    [PrimaryEntityIndex]
    public string value;

    [PrimaryEntityIndex]
    public string value2;
}
