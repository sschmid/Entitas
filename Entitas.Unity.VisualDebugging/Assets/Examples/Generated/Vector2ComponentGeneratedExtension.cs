namespace Entitas {
    public partial class Entity {
        public Vector2Component vector2 { get { return (Vector2Component)GetComponent(ComponentIds.Vector2); } }

        public bool hasVector2 { get { return HasComponent(ComponentIds.Vector2); } }

        public Entity AddVector2(UnityEngine.Vector2 newVector2) {
            var componentPool = GetComponentPool(ComponentIds.Vector2);
            var component = (Vector2Component)(componentPool.Count > 0 ? componentPool.Pop() : new Vector2Component());
            component.vector2 = newVector2;
            return AddComponent(ComponentIds.Vector2, component);
        }

        public Entity ReplaceVector2(UnityEngine.Vector2 newVector2) {
            var componentPool = GetComponentPool(ComponentIds.Vector2);
            var component = (Vector2Component)(componentPool.Count > 0 ? componentPool.Pop() : new Vector2Component());
            component.vector2 = newVector2;
            ReplaceComponent(ComponentIds.Vector2, component);
            return this;
        }

        public Entity RemoveVector2() {
            return RemoveComponent(ComponentIds.Vector2);;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherVector2;

        public static IMatcher Vector2 {
            get {
                if (_matcherVector2 == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.Vector2);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherVector2 = matcher;
                }

                return _matcherVector2;
            }
        }
    }
}
