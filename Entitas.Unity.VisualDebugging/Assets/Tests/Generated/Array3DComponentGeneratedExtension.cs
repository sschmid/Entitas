namespace Entitas {
    public partial class Entity {
        public Array3DComponent array3D { get { return (Array3DComponent)GetComponent(ComponentIds.Array3D); } }

        public bool hasArray3D { get { return HasComponent(ComponentIds.Array3D); } }

        public Entity AddArray3D(Array3DComponent component) {
            return AddComponent(ComponentIds.Array3D, component);
        }

        public Entity AddArray3D(int[,,] newArray3d) {
            var component = new Array3DComponent();
            component.array3d = newArray3d;
            return AddArray3D(component);
        }

        public Entity ReplaceArray3D(int[,,] newArray3d) {
            Array3DComponent component;
            if (hasArray3D) {
                component = array3D;
            } else {
                component = new Array3DComponent();
            }
            component.array3d = newArray3d;
            return ReplaceComponent(ComponentIds.Array3D, component);
        }

        public Entity RemoveArray3D() {
            return RemoveComponent(ComponentIds.Array3D);
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
