using System;
using Entitas;
using Entitas.CodeGenerator;
using Entitas.Serialization;

[SingleEntity]
public class UserComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return new ComponentInfo(
                typeof(UserComponent).ToCompilableString(),
                new [] {
                    new ComponentFieldInfo("System.DateTime", "timestamp"),
                    new ComponentFieldInfo("bool", "isLoggedIn")
                },
                new string[0],
                true,
                "is",
                true,
                true
            );
        }
    }

    public DateTime timestamp;
    public bool isLoggedIn;
    public static string extensions =
        @"namespace Entitas {
    public partial class Entity {
        public UserComponent user { get { return (UserComponent)GetComponent(ComponentIds.User); } }

        public bool hasUser { get { return HasComponent(ComponentIds.User); } }

        public Entity AddUser(System.DateTime newTimestamp, bool newIsLoggedIn) {
            var componentPool = GetComponentPool(ComponentIds.User);
            var component = (UserComponent)(componentPool.Count > 0 ? componentPool.Pop() : new UserComponent());
            component.timestamp = newTimestamp;
            component.isLoggedIn = newIsLoggedIn;
            return AddComponent(ComponentIds.User, component);
        }

        public Entity ReplaceUser(System.DateTime newTimestamp, bool newIsLoggedIn) {
            var componentPool = GetComponentPool(ComponentIds.User);
            var component = (UserComponent)(componentPool.Count > 0 ? componentPool.Pop() : new UserComponent());
            component.timestamp = newTimestamp;
            component.isLoggedIn = newIsLoggedIn;
            ReplaceComponent(ComponentIds.User, component);
            return this;
        }

        public Entity RemoveUser() {
            return RemoveComponent(ComponentIds.User);
        }
    }

    public partial class Pool {
        public Entity userEntity { get { return GetGroup(Matcher.User).GetSingleEntity(); } }

        public UserComponent user { get { return userEntity.user; } }

        public bool hasUser { get { return userEntity != null; } }

        public Entity SetUser(System.DateTime newTimestamp, bool newIsLoggedIn) {
            if (hasUser) {
                throw new EntitasException(""Could not set user!\n"" + this + "" already has an entity with UserComponent!"",
                    ""You should check if the pool already has a userEntity before setting it or use pool.ReplaceUser()."");
            }
            var entity = CreateEntity();
            entity.AddUser(newTimestamp, newIsLoggedIn);
            return entity;
        }

        public Entity ReplaceUser(System.DateTime newTimestamp, bool newIsLoggedIn) {
            var entity = userEntity;
            if (entity == null) {
                entity = SetUser(newTimestamp, newIsLoggedIn);
            } else {
                entity.ReplaceUser(newTimestamp, newIsLoggedIn);
            }

            return entity;
        }

        public void RemoveUser() {
            DestroyEntity(userEntity);
        }
    }

    public partial class Matcher {
        static IMatcher _matcherUser;

        public static IMatcher User {
            get {
                if (_matcherUser == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.User);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherUser = matcher;
                }

                return _matcherUser;
            }
        }
    }
}
";
}
