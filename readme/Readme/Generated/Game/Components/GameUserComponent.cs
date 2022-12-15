public partial class GameContext
{
    public GameEntity UserEntity => GetGroup(GameMatcher.User).GetSingleEntity();
    public UserComponent User => UserEntity.User;
    public bool HasUser => UserEntity != null;

    public GameEntity SetUser(string newName, int newAge)
    {
        if (HasUser)
            throw new Entitas.EntitasException($"Could not set User!\n{this} already has an entity with UserComponent!",
                "You should check if the context already has a UserEntity before setting it or use context.ReplaceUser().");
        var entity = CreateEntity();
        entity.AddUser(newName, newAge);
        return entity;
    }

    public GameEntity ReplaceUser(string newName, int newAge)
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

public partial class GameEntity {

    public UserComponent user { get { return (UserComponent)GetComponent(GameComponentsLookup.User); } }
    public bool hasUser { get { return HasComponent(GameComponentsLookup.User); } }

    public void AddUser(string newName, int newAge) {
        var index = GameComponentsLookup.User;
        var component = (UserComponent)CreateComponent(index, typeof(UserComponent));
        component.Name = newName;
        component.Age = newAge;
        AddComponent(index, component);
    }

    public void ReplaceUser(string newName, int newAge) {
        var index = GameComponentsLookup.User;
        var component = (UserComponent)CreateComponent(index, typeof(UserComponent));
        component.Name = newName;
        component.Age = newAge;
        ReplaceComponent(index, component);
    }

    public void RemoveUser() {
        RemoveComponent(GameComponentsLookup.User);
    }
}

public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherUser;

    public static Entitas.IMatcher<GameEntity> User {
        get {
            if (_matcherUser == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.User);
                matcher.ComponentNames = GameComponentsLookup.componentNames;
                _matcherUser = matcher;
            }

            return _matcherUser;
        }
    }
}
