using Entitas;
using Entitas.VisualDebugging.Unity;

[Game, DontDrawComponent]
public class MyDontDrawComponent : IComponent
{
    public MySimpleObject Value;
}
