using Entitas;
using Entitas.Plugins.Attributes;

[Context("Test1"), Event(EventTarget.Self, EventType.Added, 1)]
public sealed class FlagEntityEventComponent : IComponent { }
