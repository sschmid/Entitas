using Entitas;
using Entitas.Plugins.Attributes;

[Context("Test1"), Context("Test2"), Event(EventTarget.Any)]
public sealed class MultipleContextStandardEventComponent : IComponent
{
    public string value;
}
