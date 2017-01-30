using Entitas;
using Entitas.CodeGenerator.Api;

[Context("Test"), Context("Test2")]
public sealed class Test2ContextComponent : IComponent {
    public string value;
}
