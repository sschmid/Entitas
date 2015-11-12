using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public AnArrayComponent anArray { get { return (AnArrayComponent)GetComponent(ComponentIds.AnArray); } }

        public bool hasAnArray { get { return HasComponent(ComponentIds.AnArray); } }

        static readonly Stack<AnArrayComponent> _anArrayComponentPool = new Stack<AnArrayComponent>();

        public static void ClearAnArrayComponentPool() {
            _anArrayComponentPool.Clear();
        }

        public Entity AddAnArray(string[] newArray) {
            var component = _anArrayComponentPool.Count > 0 ? _anArrayComponentPool.Pop() : new AnArrayComponent();
            component.array = newArray;
            return AddComponent(ComponentIds.AnArray, component);
        }

        public Entity ReplaceAnArray(string[] newArray) {
            var previousComponent = hasAnArray ? anArray : null;
            var component = _anArrayComponentPool.Count > 0 ? _anArrayComponentPool.Pop() : new AnArrayComponent();
            component.array = newArray;
            ReplaceComponent(ComponentIds.AnArray, component);
            if (previousComponent != null) {
                _anArrayComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveAnArray() {
            var component = anArray;
            RemoveComponent(ComponentIds.AnArray);
            _anArrayComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherAnArray;

        public static IMatcher AnArray {
            get {
                if (_matcherAnArray == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.AnArray);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherAnArray = matcher;
                }

                return _matcherAnArray;
            }
        }
    }
}
