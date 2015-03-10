using Entitas;
using System.Collections.Generic;

public class HashSetComponent : IComponent {
    public HashSet<int> numbers;
    public static string extensions = @"namespace Entitas {
    public partial class Entity {
        public HashSetComponent hashSet { get { return (HashSetComponent)GetComponent(ComponentIds.HashSet); } }

        public bool hasHashSet { get { return HasComponent(ComponentIds.HashSet); } }

        public void AddHashSet(HashSetComponent component) {
            AddComponent(ComponentIds.HashSet, component);
        }

        public void AddHashSet(System.Collections.Generic.HashSet<int> newNumbers) {
            var component = new HashSetComponent();
            component.numbers = newNumbers;
            AddHashSet(component);
        }

        public void ReplaceHashSet(System.Collections.Generic.HashSet<int> newNumbers) {
            HashSetComponent component;
            if (hasHashSet) {
                WillRemoveComponent(ComponentIds.HashSet);
                component = hashSet;
            } else {
                component = new HashSetComponent();
            }
            component.numbers = newNumbers;
            ReplaceComponent(ComponentIds.HashSet, component);
        }

        public void RemoveHashSet() {
            RemoveComponent(ComponentIds.HashSet);
        }
    }

    public static partial class Matcher {
        static AllOfMatcher _matcherHashSet;

        public static AllOfMatcher HashSet {
            get {
                if (_matcherHashSet == null) {
                    _matcherHashSet = Matcher.AllOf(new [] { ComponentIds.HashSet });
                }

                return _matcherHashSet;
            }
        }
    }
}";
}

