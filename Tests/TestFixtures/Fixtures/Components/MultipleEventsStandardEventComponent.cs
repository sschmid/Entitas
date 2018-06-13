using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Context("Test2"), Event(EventTarget.Any, EventType.Added, 1), Event(EventTarget.Self, EventType.Removed, 2)]
public sealed class MultipleEventsStandardEventComponent : IComponent {
    public string value;
}
