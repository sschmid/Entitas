using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Unique, FlagPrefix("My")]
public sealed class CustomPrefixFlagComponent : IComponent { }
