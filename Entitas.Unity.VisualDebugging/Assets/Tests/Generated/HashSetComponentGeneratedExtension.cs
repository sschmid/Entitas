namespace Entitas {
    public partial class Entity {
        public HashSetComponent hashSet { get { return (HashSetComponent)GetComponent(ComponentIds.HashSet); } }

        public bool hasHashSet { get { return HasComponent(ComponentIds.HashSet); } }

        public Entity AddHashSet(HashSetComponent component) {
            return AddComponent(ComponentIds.HashSet, component);
        }

        public Entity AddHashSet(System.Collections.Generic.HashSet<string> newHashset) {
            var component = new HashSetComponent();
            component.hashset = newHashset;
            return AddHashSet(component);
        }

        public Entity ReplaceHashSet(System.Collections.Generic.HashSet<string> newHashset) {
            HashSetComponent component;
            if (hasHashSet) {
                WillRemoveComponent(ComponentIds.HashSet);
                component = hashSet;
            } else {
                component = new HashSetComponent();
            }
            component.hashset = newHashset;
            return ReplaceComponent(ComponentIds.HashSet, component);
        }

        public Entity RemoveHashSet() {
            return RemoveComponent(ComponentIds.HashSet);
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
