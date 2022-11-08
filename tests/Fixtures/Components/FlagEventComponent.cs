using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test1"), Event(EventTarget.Any, EventType.Removed)]
public sealed class FlagEventComponent : IComponent { }
