using Entitas.Api;
using Entitas.CodeGenerator.Api;

[Test, Unique, CustomPrefix("My")]
public sealed class CustomPrefixFlagComponent : IComponent {
}
