using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public UnityObjectComponent unityObject { get { return (UnityObjectComponent)GetComponent(ComponentIds.UnityObject); } }

        public bool hasUnityObject { get { return HasComponent(ComponentIds.UnityObject); } }

        static readonly Stack<UnityObjectComponent> _unityObjectComponentPool = new Stack<UnityObjectComponent>();

        public static void ClearUnityObjectComponentPool() {
            _unityObjectComponentPool.Clear();
        }

        public Entity AddUnityObject(UnityEngine.Object newUnityObject) {
            var component = _unityObjectComponentPool.Count > 0 ? _unityObjectComponentPool.Pop() : new UnityObjectComponent();
            component.unityObject = newUnityObject;
            return AddComponent(ComponentIds.UnityObject, component);
        }

        public Entity ReplaceUnityObject(UnityEngine.Object newUnityObject) {
            var previousComponent = hasUnityObject ? unityObject : null;
            var component = _unityObjectComponentPool.Count > 0 ? _unityObjectComponentPool.Pop() : new UnityObjectComponent();
            component.unityObject = newUnityObject;
            ReplaceComponent(ComponentIds.UnityObject, component);
            if (previousComponent != null) {
                _unityObjectComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveUnityObject() {
            var component = unityObject;
            RemoveComponent(ComponentIds.UnityObject);
            _unityObjectComponentPool.Push(component);
            return this;
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
