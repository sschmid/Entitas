using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Context("Test2"), Event(false, EventType.Added, 1), Event(true, EventType.Removed, 2)]
public sealed class MultipleEventsStandardEventComponent : IComponent {
    public string value;
}
