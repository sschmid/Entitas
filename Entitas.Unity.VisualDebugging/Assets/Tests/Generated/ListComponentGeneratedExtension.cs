namespace Entitas {
    public partial class Entity {
        public ListComponent list { get { return (ListComponent)GetComponent(ComponentIds.List); } }

        public bool hasList { get { return HasComponent(ComponentIds.List); } }

        public Entity AddList(ListComponent component) {
            return AddComponent(ComponentIds.List, component);
        }

        public Entity AddList(System.Collections.Generic.List<string> newList) {
            var component = new ListComponent();
            component.list = newList;
            return AddList(component);
        }

        public Entity ReplaceList(System.Collections.Generic.List<string> newList) {
            ListComponent component;
            if (hasList) {
                component = list;
            } else {
                component = new ListComponent();
            }
            component.list = newList;
            return ReplaceComponent(ComponentIds.List, component);
        }

        public Entity RemoveList() {
            return RemoveComponent(ComponentIds.List);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherList;

        public static AllOfMatcher List {
            get {
                if (_matcherList == null) {
                    _matcherList = new Matcher(ComponentIds.List);
                }

                return _matcherList;
            }
        }
    }
}
