namespace Entitas {
    public partial class Entity {
        public DictArrayComponent dictArray { get { return (DictArrayComponent)GetComponent(ComponentIds.DictArray); } }

        public bool hasDictArray { get { return HasComponent(ComponentIds.DictArray); } }

        public void AddDictArray(DictArrayComponent component) {
            AddComponent(ComponentIds.DictArray, component);
        }

        public void AddDictArray(System.Collections.Generic.Dictionary<int, string[]> newDict, System.Collections.Generic.Dictionary<int, string[]>[] newDictArray) {
            var component = new DictArrayComponent();
            component.dict = newDict;
            component.dictArray = newDictArray;
            AddDictArray(component);
        }

        public void ReplaceDictArray(System.Collections.Generic.Dictionary<int, string[]> newDict, System.Collections.Generic.Dictionary<int, string[]>[] newDictArray) {
            DictArrayComponent component;
            if (hasDictArray) {
                WillRemoveComponent(ComponentIds.DictArray);
                component = dictArray;
            } else {
                component = new DictArrayComponent();
            }
            component.dict = newDict;
            component.dictArray = newDictArray;
            ReplaceComponent(ComponentIds.DictArray, component);
        }

        public void RemoveDictArray() {
            RemoveComponent(ComponentIds.DictArray);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherDictArray;

        public static AllOfMatcher DictArray {
            get {
                if (_matcherDictArray == null) {
                    _matcherDictArray = new Matcher(ComponentIds.DictArray);
                }

                return _matcherDictArray;
            }
        }
    }
}
