namespace Entitas {
    public partial class Entity {
        public Vector4Component vector4 { get { return (Vector4Component)GetComponent(ComponentIds.Vector4); } }

        public bool hasVector4 { get { return HasComponent(ComponentIds.Vector4); } }

        public Entity AddVector4(Vector4Component component) {
            return AddComponent(ComponentIds.Vector4, component);
        }

        public Entity AddVector4(UnityEngine.Vector4 newVector4) {
            var component = new Vector4Component();
            component.vector4 = newVector4;
            return AddVector4(component);
        }

        public Entity ReplaceVector4(UnityEngine.Vector4 newVector4) {
            Vector4Component component;
            if (hasVector4) {
                WillRemoveComponent(ComponentIds.Vector4);
                component = vector4;
            } else {
                component = new Vector4Component();
            }
            component.vector4 = newVector4;
            return ReplaceComponent(ComponentIds.Vector4, component);
        }

        public Entity RemoveVector4() {
            return RemoveComponent(ComponentIds.Vector4);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherVector4;

        public static AllOfMatcher Vector4 {
            get {
                if (_matcherVector4 == null) {
                    _matcherVector4 = new Matcher(ComponentIds.Vector4);
                }

                return _matcherVector4;
            }
        }
    }
}
