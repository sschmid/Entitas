using Entitas;
using Entitas.CodeGenerator.Attributes;

public class PrimaryEntityIndexComponent : IComponent {

    [PrimaryEntityIndex]
    public string value;
}
