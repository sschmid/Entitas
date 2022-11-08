using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test1"), DontGenerate]
public sealed class DontGenerateMethodsComponent : IComponent { }
