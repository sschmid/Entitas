using Entitas;
using Entitas.CodeGenerator.Attributes;

[Context("Test"), DontGenerate]
public sealed class DontGenerateMethodsComponent : IComponent {
}
