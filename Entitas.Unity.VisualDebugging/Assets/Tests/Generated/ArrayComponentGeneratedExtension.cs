using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public ArrayComponent array { get { return (ArrayComponent)GetComponent(ComponentIds.Array); } }

        public bool hasArray { get { return HasComponent(ComponentIds.Array); } }

        static readonly Stack<ArrayComponent> _arrayComponentPool = new Stack<ArrayComponent>();

        public static void ClearArrayComponentPool() {
            _arrayComponentPool.Clear();
        }

        public Entity AddArray(string[] newArray) {
            var component = _arrayComponentPool.Count > 0 ? _arrayComponentPool.Pop() : new ArrayComponent();
            component.array = newArray;
            return AddComponent(ComponentIds.Array, component);
        }

        public Entity ReplaceArray(string[] newArray) {
            var previousComponent = hasArray ? array : null;
            var component = _arrayComponentPool.Count > 0 ? _arrayComponentPool.Pop() : new ArrayComponent();
            component.array = newArray;
            ReplaceComponent(ComponentIds.Array, component);
            if (previousComponent != null) {
                _arrayComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveArray() {
            var component = array;
            RemoveComponent(ComponentIds.Array);
            _arrayComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherArray;

        public static AllOfMatcher Array {
            get {
                if (_matcherArray == null) {
                    _matcherArray = new Matcher(ComponentIds.Array);
                }

                return _matcherArray;
            }
        }
    }
}
