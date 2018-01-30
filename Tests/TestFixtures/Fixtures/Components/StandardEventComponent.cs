using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Event]
public sealed class StandardEventComponent : IComponent {
    public string value;
}
