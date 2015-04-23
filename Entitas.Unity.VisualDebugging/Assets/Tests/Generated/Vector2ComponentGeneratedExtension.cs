namespace Entitas {
    public partial class Entity {
        public Vector2Component vector2 { get { return (Vector2Component)GetComponent(ComponentIds.Vector2); } }

        public bool hasVector2 { get { return HasComponent(ComponentIds.Vector2); } }

        public void AddVector2(Vector2Component component) {
            AddComponent(ComponentIds.Vector2, component);
        }

        public void AddVector2(UnityEngine.Vector2 newVector2) {
            var component = new Vector2Component();
            component.vector2 = newVector2;
            AddVector2(component);
        }

        public void ReplaceVector2(UnityEngine.Vector2 newVector2) {
            Vector2Component component;
            if (hasVector2) {
                WillRemoveComponent(ComponentIds.Vector2);
                component = vector2;
            } else {
                component = new Vector2Component();
            }
            component.vector2 = newVector2;
            ReplaceComponent(ComponentIds.Vector2, component);
        }

        public void RemoveVector2() {
            RemoveComponent(ComponentIds.Vector2);
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
