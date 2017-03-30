using Entitas;
using Entitas.CodeGenerator.Attributes;

[Context("Test"), Unique]
public sealed class UniqueStandardComponent : IComponent {
    public string value;
}
