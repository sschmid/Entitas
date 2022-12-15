public partial class InputContext
{
    public InputEntity UserEntity => GetGroup(InputMatcher.User).GetSingleEntity();
    public UserComponent User => UserEntity.User;
    public bool HasUser => UserEntity != null;

    public InputEntity SetUser(string newName, int newAge)
    {
        if (HasUser)
            throw new Entitas.EntitasException($"Could not set User!\n{this} already has an entity with UserComponent!",
                "You should check if the context already has a UserEntity before setting it or use context.ReplaceUser().");
        var entity = CreateEntity();
        entity.AddUser(newName, newAge);
        return entity;
    }

    public InputEntity ReplaceUser(string newName, int newAge)
    {
        var entity = UserEntity;
        if (entity == null)
            entity = SetUser(newName, newAge);
        else
            entity.ReplaceUser(newName, newAge);
        return entity;
    }

    public void RemoveUser() => UserEntity.Destroy();
}

public partial class InputEntity
{
    public UserComponent User => (UserComponent)GetComponent(InputComponentsLookup.User);
    public bool HasUser => HasComponent(InputComponentsLookup.User);

    public InputEntity AddUser(string newName, int newAge)
    {
        var index = InputComponentsLookup.User;
        var component = (UserComponent)CreateComponent(index, typeof(UserComponent));
        component.Name = newName;
        component.Age = newAge;
        AddComponent(index, component);
        return this;
    }

    public InputEntity ReplaceUser(string newName, int newAge)
    {
        var index = InputComponentsLookup.User;
        var component = (UserComponent)CreateComponent(index, typeof(UserComponent));
        component.Name = newName;
        component.Age = newAge;
        ReplaceComponent(index, component);
        return this;
    }

    public InputEntity RemoveUser()
    {
        RemoveComponent(InputComponentsLookup.User);
        return this;
    }
}

public partial class InputEntity : IUserEntity { }

public sealed partial class InputMatcher
{
    static Entitas.IMatcher<InputEntity> _matcherUser;

    public static Entitas.IMatcher<InputEntity> User
    {
        get
        {
            if (_matcherUser == null)
            {
                var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.User);
                matcher.ComponentNames = InputComponentsLookup.ComponentNames;
                _matcherUser = matcher;
            }

            return _matcherUser;
        }
    }
}
