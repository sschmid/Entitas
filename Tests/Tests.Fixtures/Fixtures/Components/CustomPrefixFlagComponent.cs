using Entitas;
using Entitas.CodeGenerator.Attributes;

[Context("Test"), Unique, CustomPrefix("My")]
public sealed class CustomPrefixFlagComponent : IComponent {
}
