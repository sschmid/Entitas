using System.Collections.Generic;
using Entitas;

public class ListArrayComponent : IComponent {
    public List<string>[] lists;
    public static string extensions =
        @"namespace Entitas {
    public partial class Entity {
        public ListArrayComponent listArray { get { return (ListArrayComponent)GetComponent(ComponentIds.ListArray); } }

        public bool hasListArray { get { return HasComponent(ComponentIds.ListArray); } }

        public void AddListArray(ListArrayComponent component) {
            AddComponent(ComponentIds.ListArray, component);
        }

        public void AddListArray(System.Collections.Generic.List<string>[] newLists) {
            var component = new ListArrayComponent();
            component.lists = newLists;
            AddListArray(component);
        }

        public void ReplaceListArray(System.Collections.Generic.List<string>[] newLists) {
            ListArrayComponent component;
            if (hasListArray) {
                WillRemoveComponent(ComponentIds.ListArray);
                component = listArray;
            } else {
                component = new ListArrayComponent();
            }
            component.lists = newLists;
            ReplaceComponent(ComponentIds.ListArray, component);
        }

        public void RemoveListArray() {
            RemoveComponent(ComponentIds.ListArray);
        }
    }

    public static partial class Matcher {
        static AllOfMatcher _matcherListArray;

        public static AllOfMatcher ListArray {
            get {
                if (_matcherListArray == null) {
                    _matcherListArray = Matcher.AllOf(new [] { ComponentIds.ListArray });
                }

                return _matcherListArray;
            }
        }
    }
}";
}

