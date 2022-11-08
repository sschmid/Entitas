using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test1"), DontGenerate(false)]
public sealed class DontGenerateIndexComponent : IComponent { }
