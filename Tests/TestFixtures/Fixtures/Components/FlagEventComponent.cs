using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Event(false, EventType.Removed)]
public sealed class FlagEventComponent : IComponent {
}
