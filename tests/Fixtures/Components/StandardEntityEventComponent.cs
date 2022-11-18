using Entitas;
using Entitas.Plugins.Attributes;

[Context("Test1"), Event(EventTarget.Self, EventType.Removed, 1)]
public sealed class StandardEntityEventComponent : IComponent
{
    public string value;
}
