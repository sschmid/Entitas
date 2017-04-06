using Entitas.Core;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Unique, CustomPrefix("My")]
public sealed class CustomPrefixFlagComponent : IComponent {
}
