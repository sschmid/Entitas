using Entitas;

public class ComponentA : IComponent {}
public class ComponentB : IComponent {}
public class ComponentC : IComponent {}
public class ComponentD : IComponent {}
public class ComponentE : IComponent {}
public class ComponentF : IComponent {}

public static class Component {
    public static readonly ComponentA A = new ComponentA();
    public static readonly ComponentB B = new ComponentB();
    public static readonly ComponentC C = new ComponentC();
}

public static class CID {

    public const int ComponentA = 1;
    public const int ComponentB = 2;
    public const int ComponentC = 3;
    public const int ComponentD = 4;

    public const int TotalComponents = 5;
}

public static class EntityTestExtensions {

    public static Test1Entity AddComponentA(this IEntity e) { e.AddComponent(CID.ComponentA, Component.A); return (Test1Entity)e; }
    public static Test1Entity AddComponentB(this IEntity e) { e.AddComponent(CID.ComponentB, Component.B); return (Test1Entity)e; }
    public static Test1Entity AddComponentC(this IEntity e) { e.AddComponent(CID.ComponentC, Component.C); return (Test1Entity)e; }

    public static bool HasComponentA(this IEntity e) { return e.HasComponent(CID.ComponentA); }
    public static bool HasComponentB(this IEntity e) { return e.HasComponent(CID.ComponentB); }
    public static bool HasComponentC(this IEntity e) { return e.HasComponent(CID.ComponentC); }

    public static Test1Entity RemoveComponentA(this IEntity e) { e.RemoveComponent(CID.ComponentA); return (Test1Entity)e; }
    public static Test1Entity RemoveComponentB(this IEntity e) { e.RemoveComponent(CID.ComponentB); return (Test1Entity)e; }
    public static Test1Entity RemoveComponentC(this IEntity e) { e.RemoveComponent(CID.ComponentC); return (Test1Entity)e; }

    public static ComponentA GetComponentA(this IEntity e) { return (ComponentA)e.GetComponent(CID.ComponentA); }
    public static ComponentB GetComponentB(this IEntity e) { return (ComponentB)e.GetComponent(CID.ComponentB); }
    public static ComponentC GetComponentC(this IEntity e) { return (ComponentC)e.GetComponent(CID.ComponentC); }

    public static Test1Entity ReplaceComponentA(this IEntity e, ComponentA component) { e.ReplaceComponent(CID.ComponentA, component); return (Test1Entity)e; }
    public static Test1Entity ReplaceComponentB(this IEntity e, ComponentB component) { e.ReplaceComponent(CID.ComponentB, component); return (Test1Entity)e; }
    public static Test1Entity ReplaceComponentC(this IEntity e, ComponentC component) { e.ReplaceComponent(CID.ComponentC, component); return (Test1Entity)e; }
}
