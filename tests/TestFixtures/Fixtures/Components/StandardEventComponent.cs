using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Event(EventTarget.Any)]
public sealed class StandardEventComponent : IComponent
{
    public string value;
}
