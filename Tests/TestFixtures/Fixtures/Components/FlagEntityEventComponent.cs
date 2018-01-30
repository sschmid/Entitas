using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Event(true, 1)]
public sealed class FlagEntityEventComponent : IComponent {
}
