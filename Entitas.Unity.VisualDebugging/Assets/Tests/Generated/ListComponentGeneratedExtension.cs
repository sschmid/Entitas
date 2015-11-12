using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public ListComponent list { get { return (ListComponent)GetComponent(ComponentIds.List); } }

        public bool hasList { get { return HasComponent(ComponentIds.List); } }

        static readonly Stack<ListComponent> _listComponentPool = new Stack<ListComponent>();

        public static void ClearListComponentPool() {
            _listComponentPool.Clear();
        }

        public Entity AddList(System.Collections.Generic.List<string> newList) {
            var component = _listComponentPool.Count > 0 ? _listComponentPool.Pop() : new ListComponent();
            component.list = newList;
            return AddComponent(ComponentIds.List, component);
        }

        public Entity ReplaceList(System.Collections.Generic.List<string> newList) {
            var previousComponent = hasList ? list : null;
            var component = _listComponentPool.Count > 0 ? _listComponentPool.Pop() : new ListComponent();
            component.list = newList;
            ReplaceComponent(ComponentIds.List, component);
            if (previousComponent != null) {
                _listComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveList() {
            var component = list;
            RemoveComponent(ComponentIds.List);
            _listComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherList;

        public static IMatcher List {
            get {
                if (_matcherList == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.List);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherList = matcher;
                }

                return _matcherList;
            }
        }
    }
}
