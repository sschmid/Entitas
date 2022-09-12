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

    public static TestEntity AddComponentA(this IEntity e) { e.AddComponent(CID.ComponentA, Component.A); return (TestEntity)e; }
    public static TestEntity AddComponentB(this IEntity e) { e.AddComponent(CID.ComponentB, Component.B); return (TestEntity)e; }
    public static TestEntity AddComponentC(this IEntity e) { e.AddComponent(CID.ComponentC, Component.C); return (TestEntity)e; }

    public static bool HasComponentA(this IEntity e) { return e.HasComponent(CID.ComponentA); }
    public static bool HasComponentB(this IEntity e) { return e.HasComponent(CID.ComponentB); }
    public static bool HasComponentC(this IEntity e) { return e.HasComponent(CID.ComponentC); }

    public static TestEntity RemoveComponentA(this IEntity e) { e.RemoveComponent(CID.ComponentA); return (TestEntity)e; }
    public static TestEntity RemoveComponentB(this IEntity e) { e.RemoveComponent(CID.ComponentB); return (TestEntity)e; }
    public static TestEntity RemoveComponentC(this IEntity e) { e.RemoveComponent(CID.ComponentC); return (TestEntity)e; }

    public static ComponentA GetComponentA(this IEntity e) { return (ComponentA)e.GetComponent(CID.ComponentA); }
    public static ComponentB GetComponentB(this IEntity e) { return (ComponentB)e.GetComponent(CID.ComponentB); }
    public static ComponentC GetComponentC(this IEntity e) { return (ComponentC)e.GetComponent(CID.ComponentC); }

    public static TestEntity ReplaceComponentA(this IEntity e, ComponentA component) { e.ReplaceComponent(CID.ComponentA, component); return (TestEntity)e; }
    public static TestEntity ReplaceComponentB(this IEntity e, ComponentB component) { e.ReplaceComponent(CID.ComponentB, component); return (TestEntity)e; }
    public static TestEntity ReplaceComponentC(this IEntity e, ComponentC component) { e.ReplaceComponent(CID.ComponentC, component); return (TestEntity)e; }
}
