namespace Entitas {
    public partial class Entity {
        public DictionaryComponent dictionary { get { return (DictionaryComponent)GetComponent(ComponentIds.Dictionary); } }

        public bool hasDictionary { get { return HasComponent(ComponentIds.Dictionary); } }

        public void AddDictionary(DictionaryComponent component) {
            AddComponent(ComponentIds.Dictionary, component);
        }

        public void AddDictionary(System.Collections.Generic.Dictionary<string, string> newDict) {
            var component = new DictionaryComponent();
            component.dict = newDict;
            AddDictionary(component);
        }

        public void ReplaceDictionary(System.Collections.Generic.Dictionary<string, string> newDict) {
            DictionaryComponent component;
            if (hasDictionary) {
                WillRemoveComponent(ComponentIds.Dictionary);
                component = dictionary;
            } else {
                component = new DictionaryComponent();
            }
            component.dict = newDict;
            ReplaceComponent(ComponentIds.Dictionary, component);
        }

        public void RemoveDictionary() {
            RemoveComponent(ComponentIds.Dictionary);
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
