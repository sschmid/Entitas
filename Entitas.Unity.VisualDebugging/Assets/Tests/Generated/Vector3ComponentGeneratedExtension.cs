using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public Vector3Component vector3 { get { return (Vector3Component)GetComponent(ComponentIds.Vector3); } }

        public bool hasVector3 { get { return HasComponent(ComponentIds.Vector3); } }

        static readonly Stack<Vector3Component> _vector3ComponentPool = new Stack<Vector3Component>();

        public static void ClearVector3ComponentPool() {
            _vector3ComponentPool.Clear();
        }

        public Entity AddVector3(UnityEngine.Vector3 newVector3) {
            var component = _vector3ComponentPool.Count > 0 ? _vector3ComponentPool.Pop() : new Vector3Component();
            component.vector3 = newVector3;
            return AddComponent(ComponentIds.Vector3, component);
        }

        public Entity ReplaceVector3(UnityEngine.Vector3 newVector3) {
            var previousComponent = vector3;
            var component = _vector3ComponentPool.Count > 0 ? _vector3ComponentPool.Pop() : new Vector3Component();
            component.vector3 = newVector3;
            ReplaceComponent(ComponentIds.Vector3, component);
            if (previousComponent != null) {
                _vector3ComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveVector3() {
            var component = vector3;
            RemoveComponent(ComponentIds.Vector3);
            _vector3ComponentPool.Push(component);
            return this;
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
