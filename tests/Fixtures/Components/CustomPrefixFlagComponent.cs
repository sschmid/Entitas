using Entitas;
using Entitas.Plugins.Attributes;

[Context("Test1"), Unique, FlagPrefix("My")]
public sealed class CustomPrefixFlagComponent : IComponent { }
