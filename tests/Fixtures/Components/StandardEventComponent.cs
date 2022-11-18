using Entitas;
using Entitas.Plugins.Attributes;

[Context("Test1"), Event(EventTarget.Any)]
public sealed class StandardEventComponent : IComponent
{
    public string value;
}
