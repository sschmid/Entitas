namespace Entitas {
    public partial class Entity {
        public GameObjectComponent gameObject { get { return (GameObjectComponent)GetComponent(ComponentIds.GameObject); } }

        public bool hasGameObject { get { return HasComponent(ComponentIds.GameObject); } }

        public void AddGameObject(GameObjectComponent component) {
            AddComponent(ComponentIds.GameObject, component);
        }

        public void AddGameObject(UnityEngine.GameObject newGameObject) {
            var component = new GameObjectComponent();
            component.gameObject = newGameObject;
            AddGameObject(component);
        }

        public void ReplaceGameObject(UnityEngine.GameObject newGameObject) {
            GameObjectComponent component;
            if (hasGameObject) {
                WillRemoveComponent(ComponentIds.GameObject);
                component = gameObject;
            } else {
                component = new GameObjectComponent();
            }
            component.gameObject = newGameObject;
            ReplaceComponent(ComponentIds.GameObject, component);
        }

        public void RemoveGameObject() {
            RemoveComponent(ComponentIds.GameObject);
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
