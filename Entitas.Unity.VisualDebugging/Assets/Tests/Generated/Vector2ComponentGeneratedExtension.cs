namespace Entitas {
    public partial class Entity {
        public Vector2Component vector2 { get { return (Vector2Component)GetComponent(ComponentIds.Vector2); } }

        public bool hasVector2 { get { return HasComponent(ComponentIds.Vector2); } }

        public Entity AddVector2(Vector2Component component) {
            return AddComponent(ComponentIds.Vector2, component);
        }

        public Entity AddVector2(UnityEngine.Vector2 newVector2) {
            var component = new Vector2Component();
            component.vector2 = newVector2;
            return AddVector2(component);
        }

        public Entity ReplaceVector2(UnityEngine.Vector2 newVector2) {
            Vector2Component component;
            if (hasVector2) {
                component = vector2;
            } else {
                component = new Vector2Component();
            }
            component.vector2 = newVector2;
            return ReplaceComponent(ComponentIds.Vector2, component);
        }

        public Entity RemoveVector2() {
            return RemoveComponent(ComponentIds.Vector2);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherVector2;

        public static AllOfMatcher Vector2 {
            get {
                if (_matcherVector2 == null) {
                    _matcherVector2 = new Matcher(ComponentIds.Vector2);
                }

                return _matcherVector2;
            }
        }
    }
}
