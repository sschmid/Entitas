namespace Entitas {
    public partial class Entity {
        public UserComponent user { get { return (UserComponent)GetComponent(ComponentIds.User); } }

        public bool hasUser { get { return HasComponent(ComponentIds.User); } }

        public Entity AddUser(UserComponent component) {
            return AddComponent(ComponentIds.User, component);
        }

        public Entity AddUser(string newName, int newAge) {
            var component = new UserComponent();
            component.name = newName;
            component.age = newAge;
            return AddUser(component);
        }

        public Entity ReplaceUser(string newName, int newAge) {
            UserComponent component;
            if (hasUser) {
                component = user;
            } else {
                component = new UserComponent();
            }
            component.name = newName;
            component.age = newAge;
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

        public Entity SetUser(string newName, int newAge) {
            if (hasUser) {
                throw new SingleEntityException(Matcher.User);
            }
            var entity = CreateEntity();
            entity.AddUser(newName, newAge);
            return entity;
        }

        public Entity ReplaceUser(string newName, int newAge) {
            var entity = userEntity;
            if (entity == null) {
                entity = SetUser(newName, newAge);
            } else {
                entity.ReplaceUser(newName, newAge);
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
