using System;
using Entitas;

public static class TestEntityExtension
{
    public static TestEntity AddUser(this TestEntity entity, string name, int age)
    {
        entity.AddComponent(0, new UserComponent { Name = name, Age = age });
        return entity;
    }

    public static TestEntity RemoveUser(this TestEntity entity)
    {
        entity.RemoveComponent(0);
        return entity;
    }

    public static UserComponent GetUser(this TestEntity entity)
    {
        return (UserComponent)entity.GetComponent(0);
    }
}

public static class OtherTestEntityExtension
{
    public static void AddUser(this OtherTestEntity entity, string name, int age)
    {
        entity.AddComponent(0, new UserComponent { Name = name, Age = age });
    }

    public static void RemoveUser(this OtherTestEntity entity)
    {
        entity.RemoveComponent(0);
    }

    public static UserComponent GetUser(this OtherTestEntity entity)
    {
        return (UserComponent)entity.GetComponent(0);
    }
}

public sealed class TestEntity : Entity { }

public sealed class TestContext : Context<TestEntity>
{
    static readonly Func<TestEntity> CreateEntityDelegate = () => new TestEntity();

    public TestContext(int totalComponents) :
        base(totalComponents, CreateEntityDelegate) { }

    public TestContext(int totalComponents, string[] componentNames, Type[] componentTypes) :
        this(totalComponents, 0, new ContextInfo("TestContext", componentNames, componentTypes)) { }

    public TestContext(int totalComponents, int startCreationIndex, ContextInfo contextInfo) :
        base(
            totalComponents,
            startCreationIndex,
            contextInfo,
            SafeAERC.Delegate,
            CreateEntityDelegate
        ) { }
}

public sealed class OtherTestEntity : Entity { }

public sealed class OtherTestContext : Context<OtherTestEntity>
{
    public OtherTestContext() :
        base(
            1,
            0,
            new ContextInfo(
                "OtherTestContext",
                new[] { "User" },
                new[] { typeof(UserComponent) }
            ),
            SafeAERC.Delegate,
            () => new OtherTestEntity()
        ) { }
}

public static class TestUserMatcher
{
    static IMatcher<TestEntity> _matcher;

    public static IMatcher<TestEntity> User
    {
        get
        {
            if (_matcher == null)
            {
                var matcher = (Matcher<TestEntity>)Matcher<TestEntity>.AllOf(0);
                matcher.ComponentNames = new[] { "User" };
                _matcher = matcher;
            }

            return _matcher;
        }
    }
}
