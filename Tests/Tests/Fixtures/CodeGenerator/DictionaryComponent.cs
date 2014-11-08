using Entitas;
using System.Collections.Generic;

public class DictionaryComponent : IComponent {
    public Dictionary<string, int> dict;
    public static string extensions =
        @"namespace Entitas {
    public partial class Entity {
        public DictionaryComponent dictionary { get { return (DictionaryComponent)GetComponent(ComponentIds.Dictionary); } }

        public bool hasDictionary { get { return HasComponent(ComponentIds.Dictionary); } }

        public void AddDictionary(DictionaryComponent component) {
            AddComponent(ComponentIds.Dictionary, component);
        }

        public void AddDictionary(System.Collections.Generic.Dictionary<string, int> newDict) {
            var component = new DictionaryComponent();
            component.dict = newDict;
            AddDictionary(component);
        }

        public void ReplaceDictionary(System.Collections.Generic.Dictionary<string, int> newDict) {
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

    public static partial class Matcher {
        static AllOfEntityMatcher _matcherDictionary;

        public static AllOfEntityMatcher Dictionary {
            get {
                if (_matcherDictionary == null) {
                    _matcherDictionary = EntityMatcher.AllOf(new [] { ComponentIds.Dictionary });
                }

                return _matcherDictionary;
            }
        }
    }
}";
}

