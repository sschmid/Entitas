using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public Array2DComponent array2D { get { return (Array2DComponent)GetComponent(ComponentIds.Array2D); } }

        public bool hasArray2D { get { return HasComponent(ComponentIds.Array2D); } }

        static readonly Stack<Array2DComponent> _array2DComponentPool = new Stack<Array2DComponent>();

        public static void ClearArray2DComponentPool() {
            _array2DComponentPool.Clear();
        }

        public Entity AddArray2D(int[,] newArray2d) {
            var component = _array2DComponentPool.Count > 0 ? _array2DComponentPool.Pop() : new Array2DComponent();
            component.array2d = newArray2d;
            return AddComponent(ComponentIds.Array2D, component);
        }

        public Entity ReplaceArray2D(int[,] newArray2d) {
            var previousComponent = hasArray2D ? array2D : null;
            var component = _array2DComponentPool.Count > 0 ? _array2DComponentPool.Pop() : new Array2DComponent();
            component.array2d = newArray2d;
            ReplaceComponent(ComponentIds.Array2D, component);
            if (previousComponent != null) {
                _array2DComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveArray2D() {
            var component = array2D;
            RemoveComponent(ComponentIds.Array2D);
            _array2DComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherArray2D;

        public static IMatcher Array2D {
            get {
                if (_matcherArray2D == null) {
                    _matcherArray2D = Matcher.AllOf(ComponentIds.Array2D);
                }

                return _matcherArray2D;
            }
        }
    }
}
