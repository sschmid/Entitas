using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public DictionaryComponent dictionary { get { return (DictionaryComponent)GetComponent(ComponentIds.Dictionary); } }

        public bool hasDictionary { get { return HasComponent(ComponentIds.Dictionary); } }

        static readonly Stack<DictionaryComponent> _dictionaryComponentPool = new Stack<DictionaryComponent>();

        public static void ClearDictionaryComponentPool() {
            _dictionaryComponentPool.Clear();
        }

        public Entity AddDictionary(System.Collections.Generic.Dictionary<string, string> newDict) {
            var component = _dictionaryComponentPool.Count > 0 ? _dictionaryComponentPool.Pop() : new DictionaryComponent();
            component.dict = newDict;
            return AddComponent(ComponentIds.Dictionary, component);
        }

        public Entity ReplaceDictionary(System.Collections.Generic.Dictionary<string, string> newDict) {
            var previousComponent = dictionary;
            var component = _dictionaryComponentPool.Count > 0 ? _dictionaryComponentPool.Pop() : new DictionaryComponent();
            component.dict = newDict;
            ReplaceComponent(ComponentIds.Dictionary, component);
            if (previousComponent != null) {
                _dictionaryComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveDictionary() {
            var component = dictionary;
            RemoveComponent(ComponentIds.Dictionary);
            _dictionaryComponentPool.Push(component);
            return this;
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
