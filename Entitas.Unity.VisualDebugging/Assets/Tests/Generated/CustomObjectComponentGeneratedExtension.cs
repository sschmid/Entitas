using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public CustomObjectComponent customObject { get { return (CustomObjectComponent)GetComponent(ComponentIds.CustomObject); } }

        public bool hasCustomObject { get { return HasComponent(ComponentIds.CustomObject); } }

        static readonly Stack<CustomObjectComponent> _customObjectComponentPool = new Stack<CustomObjectComponent>();

        public static void ClearCustomObjectComponentPool() {
            _customObjectComponentPool.Clear();
        }

        public Entity AddCustomObject(CustomObject newCustomObject) {
            var component = _customObjectComponentPool.Count > 0 ? _customObjectComponentPool.Pop() : new CustomObjectComponent();
            component.customObject = newCustomObject;
            return AddComponent(ComponentIds.CustomObject, component);
        }

        public Entity ReplaceCustomObject(CustomObject newCustomObject) {
            var previousComponent = hasCustomObject ? customObject : null;
            var component = _customObjectComponentPool.Count > 0 ? _customObjectComponentPool.Pop() : new CustomObjectComponent();
            component.customObject = newCustomObject;
            ReplaceComponent(ComponentIds.CustomObject, component);
            if (previousComponent != null) {
                _customObjectComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveCustomObject() {
            var component = customObject;
            RemoveComponent(ComponentIds.CustomObject);
            _customObjectComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherCustomObject;

        public static IMatcher CustomObject {
            get {
                if (_matcherCustomObject == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.CustomObject);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherCustomObject = matcher;
                }

                return _matcherCustomObject;
            }
        }
    }
}
