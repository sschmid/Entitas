using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Context("Test2"), Event(false)]
public sealed class MultipleContextStandardEventComponent : IComponent {
    public string value;
}
