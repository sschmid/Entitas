namespace Entitas {
    public partial class Entity {
        public DictArrayComponent dictArray { get { return (DictArrayComponent)GetComponent(ComponentIds.DictArray); } }

        public bool hasDictArray { get { return HasComponent(ComponentIds.DictArray); } }

        public Entity AddDictArray(DictArrayComponent component) {
            return AddComponent(ComponentIds.DictArray, component);
        }

        public Entity AddDictArray(System.Collections.Generic.Dictionary<int, string[]> newDict, System.Collections.Generic.Dictionary<int, string[]>[] newDictArray) {
            var component = new DictArrayComponent();
            component.dict = newDict;
            component.dictArray = newDictArray;
            return AddDictArray(component);
        }

        public Entity ReplaceDictArray(System.Collections.Generic.Dictionary<int, string[]> newDict, System.Collections.Generic.Dictionary<int, string[]>[] newDictArray) {
            DictArrayComponent component;
            if (hasDictArray) {
                component = dictArray;
            } else {
                component = new DictArrayComponent();
            }
            component.dict = newDict;
            component.dictArray = newDictArray;
            return ReplaceComponent(ComponentIds.DictArray, component);
        }

        public Entity RemoveDictArray() {
            return RemoveComponent(ComponentIds.DictArray);
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
