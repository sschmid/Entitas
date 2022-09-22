using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Unique, Event(EventTarget.Any)]
public sealed class UniqueEventComponent : IComponent
{
    public string value;
}
