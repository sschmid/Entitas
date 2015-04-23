namespace Entitas {
    public partial class Entity {
        public Array2DComponent array2D { get { return (Array2DComponent)GetComponent(ComponentIds.Array2D); } }

        public bool hasArray2D { get { return HasComponent(ComponentIds.Array2D); } }

        public void AddArray2D(Array2DComponent component) {
            AddComponent(ComponentIds.Array2D, component);
        }

        public void AddArray2D(int[,] newArray2d) {
            var component = new Array2DComponent();
            component.array2d = newArray2d;
            AddArray2D(component);
        }

        public void ReplaceArray2D(int[,] newArray2d) {
            Array2DComponent component;
            if (hasArray2D) {
                WillRemoveComponent(ComponentIds.Array2D);
                component = array2D;
            } else {
                component = new Array2DComponent();
            }
            component.array2d = newArray2d;
            ReplaceComponent(ComponentIds.Array2D, component);
        }

        public void RemoveArray2D() {
            RemoveComponent(ComponentIds.Array2D);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherArray2D;

        public static AllOfMatcher Array2D {
            get {
                if (_matcherArray2D == null) {
                    _matcherArray2D = new Matcher(ComponentIds.Array2D);
                }

                return _matcherArray2D;
            }
        }
    }
}
