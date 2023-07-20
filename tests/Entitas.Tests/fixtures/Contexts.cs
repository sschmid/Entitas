public static class TestEntityExtension
{
    public static void AddUser(this TestEntity entity, string name, int age)
    {
        entity.AddComponent(0, new UserComponent { Name = name, Age = age });
    }

    public static void RemoveUser(this TestEntity entity)
    {
        entity.RemoveComponent(0);
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

public sealed class TestEntity : Entitas.Entity { }

public sealed class TestContext : Entitas.Context<TestEntity>
{
    public static string[] ComponentNames;
    public static System.Type[] ComponentTypes;

    public TestContext()
        : base(
            ComponentTypes.Length,
            0,
            new Entitas.ContextInfo(
                "TestContext",
                ComponentNames,
                ComponentTypes
            ),
#if (ENTITAS_FAST_AND_UNSAFE)
            Entitas.UnsafeAERC.Delegate,
#else
            Entitas.SafeAERC.Delegate,
#endif
            () => new TestEntity()
        ) { }
}

public sealed class OtherTestEntity : Entitas.Entity { }

public sealed class OtherTestContext : Entitas.Context<OtherTestEntity>
{
    public static string[] ComponentNames;
    public static System.Type[] ComponentTypes;

    public OtherTestContext()
        : base(
            ComponentTypes.Length,
            0,
            new Entitas.ContextInfo(
                "OtherTestContext",
                ComponentNames,
                ComponentTypes
            ),
#if (ENTITAS_FAST_AND_UNSAFE)
            Entitas.UnsafeAERC.Delegate,
#else
            Entitas.SafeAERC.Delegate,
#endif
            () => new OtherTestEntity()
        ) { }
}

public static class TestUserMatcher
{
    static Entitas.IMatcher<TestEntity> _matcher;

    public static Entitas.IMatcher<TestEntity> User
    {
        get
        {
            if (_matcher == null)
            {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(0);
                matcher.ComponentNames = TestContext.ComponentNames;
                _matcher = matcher;
            }

            return _matcher;
        }
    }
}
