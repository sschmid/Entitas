using Entitas;
using Entitas.Plugins.Attributes;

[Context("Test1"), Context("Test2"), Event(EventTarget.Any, EventType.Added, 1), Event(EventTarget.Self, EventType.Removed, 2)]
public sealed class MultipleEventsStandardEventComponent : IComponent
{
    public string Value;
}
