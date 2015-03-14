namespace Entitas {
    public partial class Entity {
        public NameComponent name { get { return (NameComponent)GetComponent(ComponentIds.Name); } }

        public bool hasName { get { return HasComponent(ComponentIds.Name); } }

        public void AddName(NameComponent component) {
            AddComponent(ComponentIds.Name, component);
        }

        public void AddName(string newName) {
            var component = new NameComponent();
            component.name = newName;
            AddName(component);
        }

        public void ReplaceName(string newName) {
            NameComponent component;
            if (hasName) {
                WillRemoveComponent(ComponentIds.Name);
                component = name;
            } else {
                component = new NameComponent();
            }
            component.name = newName;
            ReplaceComponent(ComponentIds.Name, component);
        }

        public void RemoveName() {
            RemoveComponent(ComponentIds.Name);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherName;

        public static AllOfMatcher Name {
            get {
                if (_matcherName == null) {
                    _matcherName = new Matcher(ComponentIds.Name);
                }

                return _matcherName;
            }
        }
    }
}