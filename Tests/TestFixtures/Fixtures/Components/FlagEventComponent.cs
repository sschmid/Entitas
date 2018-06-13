using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Event(EventTarget.Any, EventType.Removed)]
public sealed class FlagEventComponent : IComponent {
}
