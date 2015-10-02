using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public MyStringComponent myString { get { return (MyStringComponent)GetComponent(ComponentIds.MyString); } }

        public bool hasMyString { get { return HasComponent(ComponentIds.MyString); } }

        static readonly Stack<MyStringComponent> _myStringComponentPool = new Stack<MyStringComponent>();

        public static void ClearMyStringComponentPool() {
            _myStringComponentPool.Clear();
        }

        public Entity AddMyString(string newMyString) {
            var component = _myStringComponentPool.Count > 0 ? _myStringComponentPool.Pop() : new MyStringComponent();
            component.myString = newMyString;
            return AddComponent(ComponentIds.MyString, component);
        }

        public Entity ReplaceMyString(string newMyString) {
            var previousComponent = hasMyString ? myString : null;
            var component = _myStringComponentPool.Count > 0 ? _myStringComponentPool.Pop() : new MyStringComponent();
            component.myString = newMyString;
            ReplaceComponent(ComponentIds.MyString, component);
            if (previousComponent != null) {
                _myStringComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveMyString() {
            var component = myString;
            RemoveComponent(ComponentIds.MyString);
            _myStringComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherMyString;

        public static IMatcher MyString {
            get {
                if (_matcherMyString == null) {
                    _matcherMyString = Matcher.AllOf(ComponentIds.MyString);
                }

                return _matcherMyString;
            }
        }
    }
}
