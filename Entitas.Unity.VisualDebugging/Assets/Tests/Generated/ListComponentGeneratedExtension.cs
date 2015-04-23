namespace Entitas {
    public partial class Entity {
        public ListComponent list { get { return (ListComponent)GetComponent(ComponentIds.List); } }

        public bool hasList { get { return HasComponent(ComponentIds.List); } }

        public void AddList(ListComponent component) {
            AddComponent(ComponentIds.List, component);
        }

        public void AddList(System.Collections.Generic.List<string> newList) {
            var component = new ListComponent();
            component.list = newList;
            AddList(component);
        }

        public void ReplaceList(System.Collections.Generic.List<string> newList) {
            ListComponent component;
            if (hasList) {
                WillRemoveComponent(ComponentIds.List);
                component = list;
            } else {
                component = new ListComponent();
            }
            component.list = newList;
            ReplaceComponent(ComponentIds.List, component);
        }

        public void RemoveList() {
            RemoveComponent(ComponentIds.List);
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
