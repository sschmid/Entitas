using Entitas.Api;
using Entitas.CodeGenerator.Api;

[Context("Test"), DontGenerate(false)]
public sealed class DontGenerateIndexComponent : IComponent {
}
