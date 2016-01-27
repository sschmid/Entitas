namespace Entitas {
    public partial class Entity {
        public UnityObjectComponent unityObject { get { return (UnityObjectComponent)GetComponent(ComponentIds.UnityObject); } }

        public bool hasUnityObject { get { return HasComponent(ComponentIds.UnityObject); } }

        public Entity AddUnityObject(UnityEngine.Object newUnityObject) {
            var componentPool = GetComponentPool(ComponentIds.UnityObject);
            var component = (UnityObjectComponent)(componentPool.Count > 0 ? componentPool.Pop() : new UnityObjectComponent());
            component.unityObject = newUnityObject;
            return AddComponent(ComponentIds.UnityObject, component);
        }

        public Entity ReplaceUnityObject(UnityEngine.Object newUnityObject) {
            var componentPool = GetComponentPool(ComponentIds.UnityObject);
            var component = (UnityObjectComponent)(componentPool.Count > 0 ? componentPool.Pop() : new UnityObjectComponent());
            component.unityObject = newUnityObject;
            ReplaceComponent(ComponentIds.UnityObject, component);
            return this;
        }

        public Entity RemoveUnityObject() {
            return RemoveComponent(ComponentIds.UnityObject);;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherUnityObject;

        public static IMatcher UnityObject {
            get {
                if (_matcherUnityObject == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.UnityObject);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherUnityObject = matcher;
                }

                return _matcherUnityObject;
            }
        }
    }
}
