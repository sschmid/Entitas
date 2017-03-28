using Entitas;
using Entitas.CodeGenerator.Attributes;

[Context("Test"), DontGenerate(false)]
public sealed class DontGenerateIndexComponent : IComponent {
}
