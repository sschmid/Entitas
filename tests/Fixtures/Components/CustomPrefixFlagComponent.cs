using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test1"), Unique, FlagPrefix("My")]
public sealed class CustomPrefixFlagComponent : IComponent { }
