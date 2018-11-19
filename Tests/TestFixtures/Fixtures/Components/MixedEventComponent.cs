using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Event(EventTarget.Any), Event(EventTarget.Self)]
public sealed class MixedEventComponent : IComponent {
    public string value;
}
