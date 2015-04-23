namespace Entitas {
    public partial class Entity {
        public ArrayComponent array { get { return (ArrayComponent)GetComponent(ComponentIds.Array); } }

        public bool hasArray { get { return HasComponent(ComponentIds.Array); } }

        public void AddArray(ArrayComponent component) {
            AddComponent(ComponentIds.Array, component);
        }

        public void AddArray(string[] newArray) {
            var component = new ArrayComponent();
            component.array = newArray;
            AddArray(component);
        }

        public void ReplaceArray(string[] newArray) {
            ArrayComponent component;
            if (hasArray) {
                WillRemoveComponent(ComponentIds.Array);
                component = array;
            } else {
                component = new ArrayComponent();
            }
            component.array = newArray;
            ReplaceComponent(ComponentIds.Array, component);
        }

        public void RemoveArray() {
            RemoveComponent(ComponentIds.Array);
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
