using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), DontGenerate(false)]
public sealed class DontGenerateIndexComponent : IComponent { }
