namespace Entitas {
    public partial class Entity {
        public GameObjectComponent gameObject { get { return (GameObjectComponent)GetComponent(ComponentIds.GameObject); } }

        public bool hasGameObject { get { return HasComponent(ComponentIds.GameObject); } }

        public Entity AddGameObject(GameObjectComponent component) {
            return AddComponent(ComponentIds.GameObject, component);
        }

        public Entity AddGameObject(UnityEngine.GameObject newGameObject) {
            var component = new GameObjectComponent();
            component.gameObject = newGameObject;
            return AddGameObject(component);
        }

        public Entity ReplaceGameObject(UnityEngine.GameObject newGameObject) {
            GameObjectComponent component;
            if (hasGameObject) {
                component = gameObject;
            } else {
                component = new GameObjectComponent();
            }
            component.gameObject = newGameObject;
            return ReplaceComponent(ComponentIds.GameObject, component);
        }

        public Entity RemoveGameObject() {
            return RemoveComponent(ComponentIds.GameObject);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherGameObject;

        public static AllOfMatcher GameObject {
            get {
                if (_matcherGameObject == null) {
                    _matcherGameObject = new Matcher(ComponentIds.GameObject);
                }

                return _matcherGameObject;
            }
        }
    }
}
