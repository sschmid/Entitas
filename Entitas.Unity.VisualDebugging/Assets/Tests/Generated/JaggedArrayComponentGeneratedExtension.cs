namespace Entitas {
    public partial class Entity {
        public JaggedArrayComponent jaggedArray { get { return (JaggedArrayComponent)GetComponent(ComponentIds.JaggedArray); } }

        public bool hasJaggedArray { get { return HasComponent(ComponentIds.JaggedArray); } }

        public void AddJaggedArray(JaggedArrayComponent component) {
            AddComponent(ComponentIds.JaggedArray, component);
        }

        public void AddJaggedArray(string[][] newJaggedArray) {
            var component = new JaggedArrayComponent();
            component.jaggedArray = newJaggedArray;
            AddJaggedArray(component);
        }

        public void ReplaceJaggedArray(string[][] newJaggedArray) {
            JaggedArrayComponent component;
            if (hasJaggedArray) {
                WillRemoveComponent(ComponentIds.JaggedArray);
                component = jaggedArray;
            } else {
                component = new JaggedArrayComponent();
            }
            component.jaggedArray = newJaggedArray;
            ReplaceComponent(ComponentIds.JaggedArray, component);
        }

        public void RemoveJaggedArray() {
            RemoveComponent(ComponentIds.JaggedArray);
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
