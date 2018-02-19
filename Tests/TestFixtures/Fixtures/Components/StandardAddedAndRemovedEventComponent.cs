using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Event(false, EventType.AddedOrRemoved)]
public sealed class StandardAddedAndRemovedEventComponent : IComponent {
    public string value;
}
