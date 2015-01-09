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

        public void AddUser(UserComponent component) {
            AddComponent(ComponentIds.User, component);
        }

        public void AddUser(System.DateTime newTimestamp, bool newIsLoggedIn) {
            var component = new UserComponent();
            component.timestamp = newTimestamp;
            component.isLoggedIn = newIsLoggedIn;
            AddUser(component);
        }

        public void ReplaceUser(System.DateTime newTimestamp, bool newIsLoggedIn) {
            UserComponent component;
            if (hasUser) {
                WillRemoveComponent(ComponentIds.User);
                component = user;
            } else {
                component = new UserComponent();
            }
            component.timestamp = newTimestamp;
            component.isLoggedIn = newIsLoggedIn;
            ReplaceComponent(ComponentIds.User, component);
        }

        public void RemoveUser() {
            RemoveComponent(ComponentIds.User);
        }
    }

    public partial class Context {
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

    public static partial class Matcher {
        static AllOfMatcher _matcherUser;

        public static AllOfMatcher User {
            get {
                if (_matcherUser == null) {
                    _matcherUser = Matcher.AllOf(new [] { ComponentIds.User });
                }

                return _matcherUser;
            }
        }
    }
}";
}
