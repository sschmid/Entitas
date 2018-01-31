using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Event(false)]
public sealed class StandardEventComponent : IComponent {
    public string value;
}
