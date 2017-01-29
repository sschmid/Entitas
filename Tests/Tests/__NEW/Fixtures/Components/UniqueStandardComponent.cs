using Entitas.Api;
using Entitas.CodeGenerator.Api;

[Test, Unique]
public sealed class UniqueStandardComponent : IComponent {
    public string value;
}
