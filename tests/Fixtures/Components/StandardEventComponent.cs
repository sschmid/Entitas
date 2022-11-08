using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test1"), Event(EventTarget.Any)]
public sealed class StandardEventComponent : IComponent
{
    public string value;
}
