namespace Entitas {
    public partial class Entity {
        public ListArrayComponent listArray { get { return (ListArrayComponent)GetComponent(ComponentIds.ListArray); } }

        public bool hasListArray { get { return HasComponent(ComponentIds.ListArray); } }

        public void AddListArray(ListArrayComponent component) {
            AddComponent(ComponentIds.ListArray, component);
        }

        public void AddListArray(System.Collections.Generic.List<string>[] newListArray) {
            var component = new ListArrayComponent();
            component.listArray = newListArray;
            AddListArray(component);
        }

        public void ReplaceListArray(System.Collections.Generic.List<string>[] newListArray) {
            ListArrayComponent component;
            if (hasListArray) {
                WillRemoveComponent(ComponentIds.ListArray);
                component = listArray;
            } else {
                component = new ListArrayComponent();
            }
            component.listArray = newListArray;
            ReplaceComponent(ComponentIds.ListArray, component);
        }

        public void RemoveListArray() {
            RemoveComponent(ComponentIds.ListArray);
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
