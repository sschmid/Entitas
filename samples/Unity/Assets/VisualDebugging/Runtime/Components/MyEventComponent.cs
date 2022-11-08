using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game, Event(EventTarget.Any)]
public class MyEventComponent : IComponent
{
    public string Value;
}
