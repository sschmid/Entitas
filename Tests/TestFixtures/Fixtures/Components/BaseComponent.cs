using Entitas.CodeGeneration.Attributes;
using Entitas;

[Context("Test"), Event(EventTarget.Any), Cleanup(CleanupMode.RemoveComponent)]
public class BaseComponent : IComponent {
    public int value;
}
