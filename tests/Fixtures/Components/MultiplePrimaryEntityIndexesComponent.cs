using Entitas;
using Entitas.Plugins.Attributes;

public class MultiplePrimaryEntityIndexesComponent : IComponent
{
    [PrimaryEntityIndex]
    public string Value;

    [PrimaryEntityIndex]
    public string Value2;
}
