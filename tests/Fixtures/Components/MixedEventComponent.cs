using Entitas;
using Entitas.Plugins.Attributes;

[Context("Test1"), Event(EventTarget.Any), Event(EventTarget.Self)]
public sealed class MixedEventComponent : IComponent
{
    public string Value;
}
