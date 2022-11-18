using Entitas;
using Entitas.Plugins.Attributes;

[Context("Test1"), Unique, Event(EventTarget.Any)]
public sealed class UniqueEventComponent : IComponent
{
    public string value;
}
