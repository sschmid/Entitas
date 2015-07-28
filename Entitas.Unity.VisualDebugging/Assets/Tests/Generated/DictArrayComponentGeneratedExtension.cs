using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public DictArrayComponent dictArray { get { return (DictArrayComponent)GetComponent(ComponentIds.DictArray); } }

        public bool hasDictArray { get { return HasComponent(ComponentIds.DictArray); } }

        static readonly Stack<DictArrayComponent> _dictArrayComponentPool = new Stack<DictArrayComponent>();

        public static void ClearDictArrayComponentPool() {
            _dictArrayComponentPool.Clear();
        }

        public Entity AddDictArray(System.Collections.Generic.Dictionary<int, string[]> newDict, System.Collections.Generic.Dictionary<int, string[]>[] newDictArray) {
            var component = _dictArrayComponentPool.Count > 0 ? _dictArrayComponentPool.Pop() : new DictArrayComponent();
            component.dict = newDict;
            component.dictArray = newDictArray;
            return AddComponent(ComponentIds.DictArray, component);
        }

        public Entity ReplaceDictArray(System.Collections.Generic.Dictionary<int, string[]> newDict, System.Collections.Generic.Dictionary<int, string[]>[] newDictArray) {
            var previousComponent = hasDictArray ? dictArray : null;
            var component = _dictArrayComponentPool.Count > 0 ? _dictArrayComponentPool.Pop() : new DictArrayComponent();
            component.dict = newDict;
            component.dictArray = newDictArray;
            ReplaceComponent(ComponentIds.DictArray, component);
            if (previousComponent != null) {
                _dictArrayComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveDictArray() {
            var component = dictArray;
            RemoveComponent(ComponentIds.DictArray);
            _dictArrayComponentPool.Push(component);
            return this;
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
