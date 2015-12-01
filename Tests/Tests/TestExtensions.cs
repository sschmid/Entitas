using Entitas;
using NSpec;

public static class EntityExtensions {
    public static bool IsEnabled(this Entity entity) {
        return (bool)entity.GetType().GetField(
            "_isEnabled", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        ).GetValue(entity);
    }
}

public static class TestExtensions {
    public static void Fail(this nspec spec) {
        "but did".should_be("should not execute");
    }

    public static Entity CreateEntity(this nspec spec) {
        return new Entity(CID.NumComponents);
    }
}

public static class EntityTestExtensions {
    public static void AddComponentA(this Entity e) {
        e.AddComponent(CID.ComponentA, Component.A);
    }

    public static void AddComponentB(this Entity e) {
        e.AddComponent(CID.ComponentB, Component.B);
    }

    public static void AddComponentC(this Entity e) {
        e.AddComponent(CID.ComponentC, Component.C);
    }

    public static void AddComponentD(this Entity e) {
        e.AddComponent(CID.ComponentD, Component.D);
    }

    public static void AddComponentE(this Entity e) {
        e.AddComponent(CID.ComponentE, Component.E);
    }

    public static void AddComponentF(this Entity e) {
        e.AddComponent(CID.ComponentF, Component.F);
    }

    public static bool HasComponentA(this Entity e) {
        return e.HasComponent(CID.ComponentA);
    }

    public static bool HasComponentB(this Entity e) {
        return e.HasComponent(CID.ComponentB);
    }

    public static bool HasComponentC(this Entity e) {
        return e.HasComponent(CID.ComponentC);
    }

    public static void RemoveComponentA(this Entity e) {
        e.RemoveComponent(CID.ComponentA);
    }

    public static void RemoveComponentB(this Entity e) {
        e.RemoveComponent(CID.ComponentB);
    }

    public static void RemoveComponentC(this Entity e) {
        e.RemoveComponent(CID.ComponentC);
    }

    public static ComponentA GetComponentA(this Entity e) {
        return (ComponentA)e.GetComponent(CID.ComponentA);
    }

    public static ComponentB GetComponentB(this Entity e) {
        return (ComponentB)e.GetComponent(CID.ComponentB);
    }

    public static ComponentC GetComponentC(this Entity e) {
        return (ComponentC)e.GetComponent(CID.ComponentC);
    }

    public static void ReplaceComponentA(this Entity e, ComponentA component) {
        e.ReplaceComponent(CID.ComponentA, component);
    }

    public static void ReplaceComponentB(this Entity e, ComponentB component) {
        e.ReplaceComponent(CID.ComponentB, component);
    }

    public static void ReplaceComponentC(this Entity e, ComponentC component) {
        e.ReplaceComponent(CID.ComponentC, component);
    }
}