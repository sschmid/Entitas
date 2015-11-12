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
            var previousComponent = hasMyInt ? myInt : null;
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
        static IMatcher _matcherMyInt;

        public static IMatcher MyInt {
            get {
                if (_matcherMyInt == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.MyInt);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherMyInt = matcher;
                }

                return _matcherMyInt;
            }
        }
    }
}
