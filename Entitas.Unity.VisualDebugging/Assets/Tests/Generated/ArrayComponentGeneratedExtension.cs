namespace Entitas {
    public partial class Entity {
        public ArrayComponent array { get { return (ArrayComponent)GetComponent(ComponentIds.Array); } }

        public bool hasArray { get { return HasComponent(ComponentIds.Array); } }

        public Entity AddArray(ArrayComponent component) {
            return AddComponent(ComponentIds.Array, component);
        }

        public Entity AddArray(string[] newArray) {
            var component = new ArrayComponent();
            component.array = newArray;
            return AddArray(component);
        }

        public Entity ReplaceArray(string[] newArray) {
            ArrayComponent component;
            if (hasArray) {
                component = array;
            } else {
                component = new ArrayComponent();
            }
            component.array = newArray;
            return ReplaceComponent(ComponentIds.Array, component);
        }

        public Entity RemoveArray() {
            return RemoveComponent(ComponentIds.Array);
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
