namespace Entitas {
    public partial class Entity {
        public UnityObjectComponent unityObject { get { return (UnityObjectComponent)GetComponent(ComponentIds.UnityObject); } }

        public bool hasUnityObject { get { return HasComponent(ComponentIds.UnityObject); } }

        public void AddUnityObject(UnityObjectComponent component) {
            AddComponent(ComponentIds.UnityObject, component);
        }

        public void AddUnityObject(UnityEngine.Object newUnityObject) {
            var component = new UnityObjectComponent();
            component.unityObject = newUnityObject;
            AddUnityObject(component);
        }

        public void ReplaceUnityObject(UnityEngine.Object newUnityObject) {
            UnityObjectComponent component;
            if (hasUnityObject) {
                WillRemoveComponent(ComponentIds.UnityObject);
                component = unityObject;
            } else {
                component = new UnityObjectComponent();
            }
            component.unityObject = newUnityObject;
            ReplaceComponent(ComponentIds.UnityObject, component);
        }

        public void RemoveUnityObject() {
            RemoveComponent(ComponentIds.UnityObject);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherUnityObject;

        public static AllOfMatcher UnityObject {
            get {
                if (_matcherUnityObject == null) {
                    _matcherUnityObject = new Matcher(ComponentIds.UnityObject);
                }

                return _matcherUnityObject;
            }
        }
    }
}
