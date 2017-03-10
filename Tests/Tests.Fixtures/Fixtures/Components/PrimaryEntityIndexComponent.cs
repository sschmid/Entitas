using Entitas;
using Entitas.CodeGenerator.Api;

public class PrimaryEntityIndexComponent : IComponent {

    [PrimaryEntityIndex]
    public string value;
}
