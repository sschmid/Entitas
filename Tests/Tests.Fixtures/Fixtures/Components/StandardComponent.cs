using Entitas;
using Entitas.CodeGenerator.Attributes;

[Context("Test")]
public sealed class StandardComponent : IComponent {
    public string value;
}
