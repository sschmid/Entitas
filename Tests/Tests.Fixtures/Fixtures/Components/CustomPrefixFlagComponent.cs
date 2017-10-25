using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Unique, UniquePrefix("My")]
public sealed class CustomPrefixFlagComponent : IComponent {
}
