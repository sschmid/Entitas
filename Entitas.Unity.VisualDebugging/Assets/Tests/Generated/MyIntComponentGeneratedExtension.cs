using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public MyIntComponent myInt { get { return (MyIntComponent)GetComponent(ComponentIds.MyInt); } }

        public bool hasMyInt { get { return HasComponent(ComponentIds.MyInt); } }

        static readonly Stack<MyIntComponent> _myIntComponentPool = new Stack<MyIntComponent>();

        public static void ClearMyIntComponentPool() {
            _myIntComponentPool.Clear();
        }

        public Entity AddMyInt(int newMyInt) {
            var component = _myIntComponentPool.Count > 0 ? _myIntComponentPool.Pop() : new MyIntComponent();
            component.myInt = newMyInt;
            return AddComponent(ComponentIds.MyInt, component);
        }

        public Entity ReplaceMyInt(int newMyInt) {
            var previousComponent = myInt;
            var component = _myIntComponentPool.Count > 0 ? _myIntComponentPool.Pop() : new MyIntComponent();
            component.myInt = newMyInt;
            ReplaceComponent(ComponentIds.MyInt, component);
            if (previousComponent != null) {
                _myIntComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveMyInt() {
            var component = myInt;
            RemoveComponent(ComponentIds.MyInt);
            _myIntComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherMyInt;

        public static AllOfMatcher MyInt {
            get {
                if (_matcherMyInt == null) {
                    _matcherMyInt = new Matcher(ComponentIds.MyInt);
                }

                return _matcherMyInt;
            }
        }
    }
}
