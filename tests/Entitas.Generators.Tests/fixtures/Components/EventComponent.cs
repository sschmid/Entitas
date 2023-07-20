#nullable disable

using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

[Context(typeof(MainContext))]
[Event(EventTarget.Any, EventType.Added, 1)]
[Event(EventTarget.Any, EventType.Removed, 2)]
[Event(EventTarget.Self, EventType.Added, 3)]
[Event(EventTarget.Self, EventType.Removed, 4)]
public sealed class EventComponent : IComponent
{
    public string Value;
}
