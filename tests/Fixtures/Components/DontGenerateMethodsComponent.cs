using Entitas;
using Entitas.Plugins.Attributes;

[Context("Test1"), DontGenerate]
public sealed class DontGenerateMethodsComponent : IComponent { }
