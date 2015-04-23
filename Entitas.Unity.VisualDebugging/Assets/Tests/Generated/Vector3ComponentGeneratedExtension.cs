namespace Entitas {
    public partial class Entity {
        public Vector3Component vector3 { get { return (Vector3Component)GetComponent(ComponentIds.Vector3); } }

        public bool hasVector3 { get { return HasComponent(ComponentIds.Vector3); } }

        public void AddVector3(Vector3Component component) {
            AddComponent(ComponentIds.Vector3, component);
        }

        public void AddVector3(UnityEngine.Vector3 newVector3) {
            var component = new Vector3Component();
            component.vector3 = newVector3;
            AddVector3(component);
        }

        public void ReplaceVector3(UnityEngine.Vector3 newVector3) {
            Vector3Component component;
            if (hasVector3) {
                WillRemoveComponent(ComponentIds.Vector3);
                component = vector3;
            } else {
                component = new Vector3Component();
            }
            component.vector3 = newVector3;
            ReplaceComponent(ComponentIds.Vector3, component);
        }

        public void RemoveVector3() {
            RemoveComponent(ComponentIds.Vector3);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherVector3;

        public static AllOfMatcher Vector3 {
            get {
                if (_matcherVector3 == null) {
                    _matcherVector3 = new Matcher(ComponentIds.Vector3);
                }

                return _matcherVector3;
            }
        }
    }
}
