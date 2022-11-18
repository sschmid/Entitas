using Entitas;
using Entitas.Plugins.Attributes;

[Context("Test1"), Event(EventTarget.Any, EventType.Removed)]
public sealed class FlagEventComponent : IComponent { }
