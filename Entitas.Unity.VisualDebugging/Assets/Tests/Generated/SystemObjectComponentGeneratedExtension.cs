using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public SystemObjectComponent systemObject { get { return (SystemObjectComponent)GetComponent(ComponentIds.SystemObject); } }

        public bool hasSystemObject { get { return HasComponent(ComponentIds.SystemObject); } }

        static readonly Stack<SystemObjectComponent> _systemObjectComponentPool = new Stack<SystemObjectComponent>();

        public static void ClearSystemObjectComponentPool() {
            _systemObjectComponentPool.Clear();
        }

        public Entity AddSystemObject(object newSystemObject) {
            var component = _systemObjectComponentPool.Count > 0 ? _systemObjectComponentPool.Pop() : new SystemObjectComponent();
            component.systemObject = newSystemObject;
            return AddComponent(ComponentIds.SystemObject, component);
        }

        public Entity ReplaceSystemObject(object newSystemObject) {
            var previousComponent = hasSystemObject ? systemObject : null;
            var component = _systemObjectComponentPool.Count > 0 ? _systemObjectComponentPool.Pop() : new SystemObjectComponent();
            component.systemObject = newSystemObject;
            ReplaceComponent(ComponentIds.SystemObject, component);
            if (previousComponent != null) {
                _systemObjectComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveSystemObject() {
            var component = systemObject;
            RemoveComponent(ComponentIds.SystemObject);
            _systemObjectComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherSystemObject;

        public static AllOfMatcher SystemObject {
            get {
                if (_matcherSystemObject == null) {
                    _matcherSystemObject = new Matcher(ComponentIds.SystemObject);
                }

                return _matcherSystemObject;
            }
        }
    }
}
