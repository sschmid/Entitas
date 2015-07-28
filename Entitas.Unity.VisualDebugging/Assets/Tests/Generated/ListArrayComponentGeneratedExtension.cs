using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public ListArrayComponent listArray { get { return (ListArrayComponent)GetComponent(ComponentIds.ListArray); } }

        public bool hasListArray { get { return HasComponent(ComponentIds.ListArray); } }

        static readonly Stack<ListArrayComponent> _listArrayComponentPool = new Stack<ListArrayComponent>();

        public static void ClearListArrayComponentPool() {
            _listArrayComponentPool.Clear();
        }

        public Entity AddListArray(System.Collections.Generic.List<string>[] newListArray) {
            var component = _listArrayComponentPool.Count > 0 ? _listArrayComponentPool.Pop() : new ListArrayComponent();
            component.listArray = newListArray;
            return AddComponent(ComponentIds.ListArray, component);
        }

        public Entity ReplaceListArray(System.Collections.Generic.List<string>[] newListArray) {
            var previousComponent = hasListArray ? listArray : null;
            var component = _listArrayComponentPool.Count > 0 ? _listArrayComponentPool.Pop() : new ListArrayComponent();
            component.listArray = newListArray;
            ReplaceComponent(ComponentIds.ListArray, component);
            if (previousComponent != null) {
                _listArrayComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveListArray() {
            var component = listArray;
            RemoveComponent(ComponentIds.ListArray);
            _listArrayComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherListArray;

        public static AllOfMatcher ListArray {
            get {
                if (_matcherListArray == null) {
                    _matcherListArray = new Matcher(ComponentIds.ListArray);
                }

                return _matcherListArray;
            }
        }
    }
}
