using Entitas;
using Entitas.CodeGenerator.Attributes;

[Context("Test"), Context("Test2")]
public sealed class Test2ContextComponent : IComponent {
    public string value;
}
