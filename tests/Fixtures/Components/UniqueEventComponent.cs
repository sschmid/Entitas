using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test1"), Unique, Event(EventTarget.Any)]
public sealed class UniqueEventComponent : IComponent
{
    public string value;
}
