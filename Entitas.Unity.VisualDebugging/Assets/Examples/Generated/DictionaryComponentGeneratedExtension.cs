namespace Entitas {
    public partial class Entity {
        public DictionaryComponent dictionary { get { return (DictionaryComponent)GetComponent(ComponentIds.Dictionary); } }

        public bool hasDictionary { get { return HasComponent(ComponentIds.Dictionary); } }

        public Entity AddDictionary(System.Collections.Generic.Dictionary<string, string> newDict) {
            var componentPool = GetComponentPool(ComponentIds.Dictionary);
            var component = (DictionaryComponent)(componentPool.Count > 0 ? componentPool.Pop() : new DictionaryComponent());
            component.dict = newDict;
            return AddComponent(ComponentIds.Dictionary, component);
        }

        public Entity ReplaceDictionary(System.Collections.Generic.Dictionary<string, string> newDict) {
            var componentPool = GetComponentPool(ComponentIds.Dictionary);
            var component = (DictionaryComponent)(componentPool.Count > 0 ? componentPool.Pop() : new DictionaryComponent());
            component.dict = newDict;
            ReplaceComponent(ComponentIds.Dictionary, component);
            return this;
        }

        public Entity RemoveDictionary() {
            return RemoveComponent(ComponentIds.Dictionary);;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherDictionary;

        public static IMatcher Dictionary {
            get {
                if (_matcherDictionary == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.Dictionary);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherDictionary = matcher;
                }

                return _matcherDictionary;
            }
        }
    }
}
