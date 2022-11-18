using Entitas;
using Entitas.Plugins.Attributes;

[Context("Test1"), DontGenerate(false)]
public sealed class DontGenerateIndexComponent : IComponent { }
