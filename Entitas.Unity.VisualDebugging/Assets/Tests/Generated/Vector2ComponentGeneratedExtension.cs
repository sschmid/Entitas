using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public Vector2Component vector2 { get { return (Vector2Component)GetComponent(ComponentIds.Vector2); } }

        public bool hasVector2 { get { return HasComponent(ComponentIds.Vector2); } }

        static readonly Stack<Vector2Component> _vector2ComponentPool = new Stack<Vector2Component>();

        public static void ClearVector2ComponentPool() {
            _vector2ComponentPool.Clear();
        }

        public Entity AddVector2(UnityEngine.Vector2 newVector2) {
            var component = _vector2ComponentPool.Count > 0 ? _vector2ComponentPool.Pop() : new Vector2Component();
            component.vector2 = newVector2;
            return AddComponent(ComponentIds.Vector2, component);
        }

        public Entity ReplaceVector2(UnityEngine.Vector2 newVector2) {
            var previousComponent = hasVector2 ? vector2 : null;
            var component = _vector2ComponentPool.Count > 0 ? _vector2ComponentPool.Pop() : new Vector2Component();
            component.vector2 = newVector2;
            ReplaceComponent(ComponentIds.Vector2, component);
            if (previousComponent != null) {
                _vector2ComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveVector2() {
            var component = vector2;
            RemoveComponent(ComponentIds.Vector2);
            _vector2ComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherVector2;

        public static IMatcher Vector2 {
            get {
                if (_matcherVector2 == null) {
                    _matcherVector2 = Matcher.AllOf(ComponentIds.Vector2);
                }

                return _matcherVector2;
            }
        }
    }
}
