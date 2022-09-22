using Entitas;
using Entitas.CodeGeneration.Attributes;

public class MultiplePrimaryEntityIndicesComponent : IComponent
{
    [PrimaryEntityIndex]
    public string value;

    [PrimaryEntityIndex]
    public string value2;
}
