using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Event(true, EventType.Added,1)]
public sealed class FlagEntityEventComponent : IComponent {
}
