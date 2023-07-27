using Entitas;

public sealed class ComponentA : IComponent { }

public sealed class ComponentB : IComponent { }

public sealed class ComponentC : IComponent { }

public static class Component
{
    public static readonly ComponentA A = new ComponentA();
    public static readonly ComponentB B = new ComponentB();
    public static readonly ComponentC C = new ComponentC();
}

public static class CID
{
    public const int ComponentA = 1;
    public const int ComponentB = 2;
    public const int ComponentC = 3;
    public const int ComponentD = 4;

    public const int TotalComponents = 5;
}

public static class TestComponentsEntityExtension
{
    public static TestEntity AddComponentA(this Entity entity)
    {
        entity.AddComponent(CID.ComponentA, Component.A);
        return (TestEntity)entity;
    }

    public static TestEntity AddComponentB(this Entity entity)
    {
        entity.AddComponent(CID.ComponentB, Component.B);
        return (TestEntity)entity;
    }

    public static TestEntity AddComponentC(this Entity entity)
    {
        entity.AddComponent(CID.ComponentC, Component.C);
        return (TestEntity)entity;
    }

    public static bool HasComponentA(this Entity entity)
    {
        return entity.HasComponent(CID.ComponentA);
    }

    public static bool HasComponentB(this Entity entity)
    {
        return entity.HasComponent(CID.ComponentB);
    }

    public static bool HasComponentC(this Entity entity)
    {
        return entity.HasComponent(CID.ComponentC);
    }

    public static TestEntity RemoveComponentA(this Entity entity)
    {
        entity.RemoveComponent(CID.ComponentA);
        return (TestEntity)entity;
    }

    public static TestEntity RemoveComponentB(this Entity entity)
    {
        entity.RemoveComponent(CID.ComponentB);
        return (TestEntity)entity;
    }

    public static TestEntity RemoveComponentC(this Entity entity)
    {
        entity.RemoveComponent(CID.ComponentC);
        return (TestEntity)entity;
    }

    public static ComponentA GetComponentA(this Entity entity)
    {
        return (ComponentA)entity.GetComponent(CID.ComponentA);
    }

    public static ComponentB GetComponentB(this Entity entity)
    {
        return (ComponentB)entity.GetComponent(CID.ComponentB);
    }

    public static ComponentC GetComponentC(this Entity entity)
    {
        return (ComponentC)entity.GetComponent(CID.ComponentC);
    }

    public static TestEntity ReplaceComponentA(this Entity entity, ComponentA component)
    {
        entity.ReplaceComponent(CID.ComponentA, component);
        return (TestEntity)entity;
    }

    public static TestEntity ReplaceComponentB(this Entity entity, ComponentB component)
    {
        entity.ReplaceComponent(CID.ComponentB, component);
        return (TestEntity)entity;
    }

    public static TestEntity ReplaceComponentC(this Entity entity, ComponentC component)
    {
        entity.ReplaceComponent(CID.ComponentC, component);
        return (TestEntity)entity;
    }
}
