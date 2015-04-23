namespace Entitas {
    public partial class Entity {
        public Array3DComponent array3D { get { return (Array3DComponent)GetComponent(ComponentIds.Array3D); } }

        public bool hasArray3D { get { return HasComponent(ComponentIds.Array3D); } }

        public void AddArray3D(Array3DComponent component) {
            AddComponent(ComponentIds.Array3D, component);
        }

        public void AddArray3D(int[,,] newArray23) {
            var component = new Array3DComponent();
            component.array23 = newArray23;
            AddArray3D(component);
        }

        public void ReplaceArray3D(int[,,] newArray23) {
            Array3DComponent component;
            if (hasArray3D) {
                WillRemoveComponent(ComponentIds.Array3D);
                component = array3D;
            } else {
                component = new Array3DComponent();
            }
            component.array23 = newArray23;
            ReplaceComponent(ComponentIds.Array3D, component);
        }

        public void RemoveArray3D() {
            RemoveComponent(ComponentIds.Array3D);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherArray3D;

        public static AllOfMatcher Array3D {
            get {
                if (_matcherArray3D == null) {
                    _matcherArray3D = new Matcher(ComponentIds.Array3D);
                }

                return _matcherArray3D;
            }
        }
    }
}
