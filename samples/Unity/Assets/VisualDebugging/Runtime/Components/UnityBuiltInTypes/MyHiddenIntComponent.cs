using Entitas;
using Entitas.VisualDebugging.Unity;

[Game, DontDrawComponent]
public class MyHiddenIntComponent : IComponent
{
    public int Value;
}
