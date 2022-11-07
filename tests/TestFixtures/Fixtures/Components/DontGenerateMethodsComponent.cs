using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), DontGenerate]
public sealed class DontGenerateMethodsComponent : IComponent { }
