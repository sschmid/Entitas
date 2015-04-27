namespace Entitas {
    public partial class Entity {
        public HashSetComponent hashSet { get { return (HashSetComponent)GetComponent(ComponentIds.HashSet); } }

        public bool hasHashSet { get { return HasComponent(ComponentIds.HashSet); } }

        public void AddHashSet(HashSetComponent component) {
            AddComponent(ComponentIds.HashSet, component);
        }

        public void AddHashSet(System.Collections.Generic.HashSet<string> newHashset) {
            var component = new HashSetComponent();
            component.hashset = newHashset;
            AddHashSet(component);
        }

        public void ReplaceHashSet(System.Collections.Generic.HashSet<string> newHashset) {
            HashSetComponent component;
            if (hasHashSet) {
                WillRemoveComponent(ComponentIds.HashSet);
                component = hashSet;
            } else {
                component = new HashSetComponent();
            }
            component.hashset = newHashset;
            ReplaceComponent(ComponentIds.HashSet, component);
        }

        public void RemoveHashSet() {
            RemoveComponent(ComponentIds.HashSet);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherHashSet;

        public static AllOfMatcher HashSet {
            get {
                if (_matcherHashSet == null) {
                    _matcherHashSet = new Matcher(ComponentIds.HashSet);
                }

                return _matcherHashSet;
            }
        }
    }
}
