namespace Entitas {
    public partial class Entity {
        public DictionaryComponent dictionary { get { return (DictionaryComponent)GetComponent(ComponentIds.Dictionary); } }

        public bool hasDictionary { get { return HasComponent(ComponentIds.Dictionary); } }

        public Entity AddDictionary(DictionaryComponent component) {
            return AddComponent(ComponentIds.Dictionary, component);
        }

        public Entity AddDictionary(System.Collections.Generic.Dictionary<string, string> newDict) {
            var component = new DictionaryComponent();
            component.dict = newDict;
            return AddDictionary(component);
        }

        public Entity ReplaceDictionary(System.Collections.Generic.Dictionary<string, string> newDict) {
            DictionaryComponent component;
            if (hasDictionary) {
                component = dictionary;
            } else {
                component = new DictionaryComponent();
            }
            component.dict = newDict;
            return ReplaceComponent(ComponentIds.Dictionary, component);
        }

        public Entity RemoveDictionary() {
            return RemoveComponent(ComponentIds.Dictionary);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherDictionary;

        public static AllOfMatcher Dictionary {
            get {
                if (_matcherDictionary == null) {
                    _matcherDictionary = new Matcher(ComponentIds.Dictionary);
                }

                return _matcherDictionary;
            }
        }
    }
}
