using System.Collections.Generic;
using Entitas;

public class ListComponent : IComponent {
    public List<string> items;
    public static string extensions =
        @"namespace Entitas {
    public partial class Entity {
        public ListComponent list { get { return (ListComponent)GetComponent(ComponentIds.List); } }

        public bool hasList { get { return HasComponent(ComponentIds.List); } }

        public void AddList(ListComponent component) {
            AddComponent(ComponentIds.List, component);
        }

        public void AddList(System.Collections.Generic.List<string> newItems) {
            var component = new ListComponent();
            component.items = newItems;
            AddList(component);
        }

        public void ReplaceList(System.Collections.Generic.List<string> newItems) {
            ListComponent component;
            if (hasList) {
                WillRemoveComponent(ComponentIds.List);
                component = list;
            } else {
                component = new ListComponent();
            }
            component.items = newItems;
            ReplaceComponent(ComponentIds.List, component);
        }

        public void RemoveList() {
            RemoveComponent(ComponentIds.List);
        }
    }

    public static partial class Matcher {
        static AllOfMatcher _matcherList;

        public static AllOfMatcher List {
            get {
                if (_matcherList == null) {
                    _matcherList = Matcher.AllOf(new [] { ComponentIds.List });
                }

                return _matcherList;
            }
        }
    }
}";
}
