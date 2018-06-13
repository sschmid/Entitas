using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Event(EventTarget.Self, EventType.Added,1)]
public sealed class FlagEntityEventComponent : IComponent {
}
