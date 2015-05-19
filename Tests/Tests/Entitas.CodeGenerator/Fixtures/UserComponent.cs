using Entitas.CodeGenerator;
using Entitas;
using System;

[SingleEntity]
public class UserComponent : IComponent {
    public DateTime timestamp;
    public bool isLoggedIn;
    public static string extensions =
        @"namespace Entitas {
    public partial class Entity {
        public UserComponent user { get { return (UserComponent)GetComponent(ComponentIds.User); } }

        public bool hasUser { get { return HasComponent(ComponentIds.User); } }

        public Entity AddUser(UserComponent component) {
            return AddComponent(ComponentIds.User, component);
        }

        public Entity AddUser(System.DateTime newTimestamp, bool newIsLoggedIn) {
            var component = new UserComponent();
            component.timestamp = newTimestamp;
            component.isLoggedIn = newIsLoggedIn;
            return AddUser(component);
        }

        public Entity ReplaceUser(System.DateTime newTimestamp, bool newIsLoggedIn) {
            UserComponent component;
            if (hasUser) {
                WillRemoveComponent(ComponentIds.User);
                component = user;
            } else {
                component = new UserComponent();
            }
            component.timestamp = newTimestamp;
            component.isLoggedIn = newIsLoggedIn;
            return ReplaceComponent(ComponentIds.User, component);
        }

        public Entity RemoveUser() {
            return RemoveComponent(ComponentIds.User);
        }
    }

    public partial class Pool {
        public Entity userEntity { get { return GetGroup(Matcher.User).GetSingleEntity(); } }

        public UserComponent user { get { return userEntity.user; } }

        public bool hasUser { get { return userEntity != null; } }

        public Entity SetUser(UserComponent component) {
            if (hasUser) {
                throw new SingleEntityException(Matcher.User);
            }
            var entity = CreateEntity();
            entity.AddUser(component);
            return entity;
        }

        public Entity SetUser(System.DateTime newTimestamp, bool newIsLoggedIn) {
            if (hasUser) {
                throw new SingleEntityException(Matcher.User);
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
        static AllOfMatcher _matcherUser;

        public static AllOfMatcher User {
            get {
                if (_matcherUser == null) {
                    _matcherUser = new Matcher(ComponentIds.User);
                }

                return _matcherUser;
            }
        }
    }
}
";
}
