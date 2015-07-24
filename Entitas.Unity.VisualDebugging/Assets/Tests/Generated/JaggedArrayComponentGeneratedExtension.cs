namespace Entitas {
    public partial class Entity {
        public JaggedArrayComponent jaggedArray { get { return (JaggedArrayComponent)GetComponent(ComponentIds.JaggedArray); } }

        public bool hasJaggedArray { get { return HasComponent(ComponentIds.JaggedArray); } }

        public Entity AddJaggedArray(JaggedArrayComponent component) {
            return AddComponent(ComponentIds.JaggedArray, component);
        }

        public Entity AddJaggedArray(string[][] newJaggedArray) {
            var component = new JaggedArrayComponent();
            component.jaggedArray = newJaggedArray;
            return AddJaggedArray(component);
        }

        public Entity ReplaceJaggedArray(string[][] newJaggedArray) {
            JaggedArrayComponent component;
            if (hasJaggedArray) {
                component = jaggedArray;
            } else {
                component = new JaggedArrayComponent();
            }
            component.jaggedArray = newJaggedArray;
            return ReplaceComponent(ComponentIds.JaggedArray, component);
        }

        public Entity RemoveJaggedArray() {
            return RemoveComponent(ComponentIds.JaggedArray);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherJaggedArray;

        public static AllOfMatcher JaggedArray {
            get {
                if (_matcherJaggedArray == null) {
                    _matcherJaggedArray = new Matcher(ComponentIds.JaggedArray);
                }

                return _matcherJaggedArray;
            }
        }
    }
}
