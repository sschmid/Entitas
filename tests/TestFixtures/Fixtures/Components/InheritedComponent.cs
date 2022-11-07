using Entitas;

public class InheritedComponent : ParentComponent { }

public class ParentComponent : IComponent
{
    public float value;
}
