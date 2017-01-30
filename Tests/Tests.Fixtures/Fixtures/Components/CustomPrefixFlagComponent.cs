using Entitas;
using Entitas.CodeGenerator.Api;

[Context("Test"), Unique, CustomPrefix("My")]
public sealed class CustomPrefixFlagComponent : IComponent {
}
