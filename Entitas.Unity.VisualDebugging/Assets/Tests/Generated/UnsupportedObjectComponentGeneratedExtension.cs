using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public UnsupportedObjectComponent unsupportedObject { get { return (UnsupportedObjectComponent)GetComponent(ComponentIds.UnsupportedObject); } }

        public bool hasUnsupportedObject { get { return HasComponent(ComponentIds.UnsupportedObject); } }

        static readonly Stack<UnsupportedObjectComponent> _unsupportedObjectComponentPool = new Stack<UnsupportedObjectComponent>();

        public static void ClearUnsupportedObjectComponentPool() {
            _unsupportedObjectComponentPool.Clear();
        }

        public Entity AddUnsupportedObject(UnsupportedObject newUnsupportedObject) {
            var component = _unsupportedObjectComponentPool.Count > 0 ? _unsupportedObjectComponentPool.Pop() : new UnsupportedObjectComponent();
            component.unsupportedObject = newUnsupportedObject;
            return AddComponent(ComponentIds.UnsupportedObject, component);
        }

        public Entity ReplaceUnsupportedObject(UnsupportedObject newUnsupportedObject) {
            var previousComponent = unsupportedObject;
            var component = _unsupportedObjectComponentPool.Count > 0 ? _unsupportedObjectComponentPool.Pop() : new UnsupportedObjectComponent();
            component.unsupportedObject = newUnsupportedObject;
            ReplaceComponent(ComponentIds.UnsupportedObject, component);
            if (previousComponent != null) {
                _unsupportedObjectComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveUnsupportedObject() {
            var component = unsupportedObject;
            RemoveComponent(ComponentIds.UnsupportedObject);
            _unsupportedObjectComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherUnsupportedObject;

        public static AllOfMatcher UnsupportedObject {
            get {
                if (_matcherUnsupportedObject == null) {
                    _matcherUnsupportedObject = new Matcher(ComponentIds.UnsupportedObject);
                }

                return _matcherUnsupportedObject;
            }
        }
    }
}
