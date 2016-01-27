namespace Entitas {
    public partial class Entity {
        public GameObjectComponent gameObject { get { return (GameObjectComponent)GetComponent(ComponentIds.GameObject); } }

        public bool hasGameObject { get { return HasComponent(ComponentIds.GameObject); } }

        public Entity AddGameObject(UnityEngine.GameObject newGameObject) {
            var componentPool = GetComponentPool(ComponentIds.GameObject);
            var component = (GameObjectComponent)(componentPool.Count > 0 ? componentPool.Pop() : new GameObjectComponent());
            component.gameObject = newGameObject;
            return AddComponent(ComponentIds.GameObject, component);
        }

        public Entity ReplaceGameObject(UnityEngine.GameObject newGameObject) {
            var componentPool = GetComponentPool(ComponentIds.GameObject);
            var component = (GameObjectComponent)(componentPool.Count > 0 ? componentPool.Pop() : new GameObjectComponent());
            component.gameObject = newGameObject;
            ReplaceComponent(ComponentIds.GameObject, component);
            return this;
        }

        public Entity RemoveGameObject() {
            return RemoveComponent(ComponentIds.GameObject);;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherGameObject;

        public static IMatcher GameObject {
            get {
                if (_matcherGameObject == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.GameObject);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherGameObject = matcher;
                }

                return _matcherGameObject;
            }
        }
    }
}
