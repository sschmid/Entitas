namespace Entitas {
    public partial class Entity {
        public ListArrayComponent listArray { get { return (ListArrayComponent)GetComponent(ComponentIds.ListArray); } }

        public bool hasListArray { get { return HasComponent(ComponentIds.ListArray); } }

        public Entity AddListArray(ListArrayComponent component) {
            return AddComponent(ComponentIds.ListArray, component);
        }

        public Entity AddListArray(System.Collections.Generic.List<string>[] newListArray) {
            var component = new ListArrayComponent();
            component.listArray = newListArray;
            return AddListArray(component);
        }

        public Entity ReplaceListArray(System.Collections.Generic.List<string>[] newListArray) {
            ListArrayComponent component;
            if (hasListArray) {
                WillRemoveComponent(ComponentIds.ListArray);
                component = listArray;
            } else {
                component = new ListArrayComponent();
            }
            component.listArray = newListArray;
            return ReplaceComponent(ComponentIds.ListArray, component);
        }

        public Entity RemoveListArray() {
            return RemoveComponent(ComponentIds.ListArray);
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
