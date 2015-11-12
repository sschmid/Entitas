using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public JaggedArrayComponent jaggedArray { get { return (JaggedArrayComponent)GetComponent(ComponentIds.JaggedArray); } }

        public bool hasJaggedArray { get { return HasComponent(ComponentIds.JaggedArray); } }

        static readonly Stack<JaggedArrayComponent> _jaggedArrayComponentPool = new Stack<JaggedArrayComponent>();

        public static void ClearJaggedArrayComponentPool() {
            _jaggedArrayComponentPool.Clear();
        }

        public Entity AddJaggedArray(string[][] newJaggedArray) {
            var component = _jaggedArrayComponentPool.Count > 0 ? _jaggedArrayComponentPool.Pop() : new JaggedArrayComponent();
            component.jaggedArray = newJaggedArray;
            return AddComponent(ComponentIds.JaggedArray, component);
        }

        public Entity ReplaceJaggedArray(string[][] newJaggedArray) {
            var previousComponent = hasJaggedArray ? jaggedArray : null;
            var component = _jaggedArrayComponentPool.Count > 0 ? _jaggedArrayComponentPool.Pop() : new JaggedArrayComponent();
            component.jaggedArray = newJaggedArray;
            ReplaceComponent(ComponentIds.JaggedArray, component);
            if (previousComponent != null) {
                _jaggedArrayComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveJaggedArray() {
            var component = jaggedArray;
            RemoveComponent(ComponentIds.JaggedArray);
            _jaggedArrayComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherJaggedArray;

        public static IMatcher JaggedArray {
            get {
                if (_matcherJaggedArray == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.JaggedArray);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherJaggedArray = matcher;
                }

                return _matcherJaggedArray;
            }
        }
    }
}
