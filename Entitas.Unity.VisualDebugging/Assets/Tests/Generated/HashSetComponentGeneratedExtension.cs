using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public HashSetComponent hashSet { get { return (HashSetComponent)GetComponent(ComponentIds.HashSet); } }

        public bool hasHashSet { get { return HasComponent(ComponentIds.HashSet); } }

        static readonly Stack<HashSetComponent> _hashSetComponentPool = new Stack<HashSetComponent>();

        public static void ClearHashSetComponentPool() {
            _hashSetComponentPool.Clear();
        }

        public Entity AddHashSet(System.Collections.Generic.HashSet<string> newHashset) {
            var component = _hashSetComponentPool.Count > 0 ? _hashSetComponentPool.Pop() : new HashSetComponent();
            component.hashset = newHashset;
            return AddComponent(ComponentIds.HashSet, component);
        }

        public Entity ReplaceHashSet(System.Collections.Generic.HashSet<string> newHashset) {
            var previousComponent = hasHashSet ? hashSet : null;
            var component = _hashSetComponentPool.Count > 0 ? _hashSetComponentPool.Pop() : new HashSetComponent();
            component.hashset = newHashset;
            ReplaceComponent(ComponentIds.HashSet, component);
            if (previousComponent != null) {
                _hashSetComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveHashSet() {
            var component = hashSet;
            RemoveComponent(ComponentIds.HashSet);
            _hashSetComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherHashSet;

        public static IMatcher HashSet {
            get {
                if (_matcherHashSet == null) {
                    _matcherHashSet = Matcher.AllOf(ComponentIds.HashSet);
                }

                return _matcherHashSet;
            }
        }
    }
}
