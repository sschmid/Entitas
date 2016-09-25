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

    public static Entity AddComponentA(this Entity e) { return e.AddComponent(CID.ComponentA, Component.A); }
    public static Entity AddComponentB(this Entity e) { return e.AddComponent(CID.ComponentB, Component.B); }
    public static Entity AddComponentC(this Entity e) { return e.AddComponent(CID.ComponentC, Component.C); }

    public static bool HasComponentA(this Entity e) { return e.HasComponent(CID.ComponentA); }
    public static bool HasComponentB(this Entity e) { return e.HasComponent(CID.ComponentB); }
    public static bool HasComponentC(this Entity e) { return e.HasComponent(CID.ComponentC); }

    public static Entity RemoveComponentA(this Entity e) { return e.RemoveComponent(CID.ComponentA); }
    public static Entity RemoveComponentB(this Entity e) { return e.RemoveComponent(CID.ComponentB); }
    public static Entity RemoveComponentC(this Entity e) { return e.RemoveComponent(CID.ComponentC); }

    public static ComponentA GetComponentA(this Entity e) { return (ComponentA)e.GetComponent(CID.ComponentA); }
    public static ComponentB GetComponentB(this Entity e) { return (ComponentB)e.GetComponent(CID.ComponentB); }
    public static ComponentC GetComponentC(this Entity e) { return (ComponentC)e.GetComponent(CID.ComponentC); }

    public static Entity ReplaceComponentA(this Entity e, ComponentA component) { return e.ReplaceComponent(CID.ComponentA, component); }
    public static Entity ReplaceComponentB(this Entity e, ComponentB component) { return e.ReplaceComponent(CID.ComponentB, component); }
    public static Entity ReplaceComponentC(this Entity e, ComponentC component) { return e.ReplaceComponent(CID.ComponentC, component); }
}