using Entitas.Core;
using Entitas.CodeGeneration.Attributes;

public class PrimaryEntityIndexComponent : IComponent {

    [PrimaryEntityIndex]
    public string value;
}
