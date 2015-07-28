using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public MyBoolComponent myBool { get { return (MyBoolComponent)GetComponent(ComponentIds.MyBool); } }

        public bool hasMyBool { get { return HasComponent(ComponentIds.MyBool); } }

        static readonly Stack<MyBoolComponent> _myBoolComponentPool = new Stack<MyBoolComponent>();

        public static void ClearMyBoolComponentPool() {
            _myBoolComponentPool.Clear();
        }

        public Entity AddMyBool(bool newMyBool) {
            var component = _myBoolComponentPool.Count > 0 ? _myBoolComponentPool.Pop() : new MyBoolComponent();
            component.myBool = newMyBool;
            return AddComponent(ComponentIds.MyBool, component);
        }

        public Entity ReplaceMyBool(bool newMyBool) {
            var previousComponent = hasMyBool ? myBool : null;
            var component = _myBoolComponentPool.Count > 0 ? _myBoolComponentPool.Pop() : new MyBoolComponent();
            component.myBool = newMyBool;
            ReplaceComponent(ComponentIds.MyBool, component);
            if (previousComponent != null) {
                _myBoolComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveMyBool() {
            var component = myBool;
            RemoveComponent(ComponentIds.MyBool);
            _myBoolComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherMyBool;

        public static AllOfMatcher MyBool {
            get {
                if (_matcherMyBool == null) {
                    _matcherMyBool = new Matcher(ComponentIds.MyBool);
                }

                return _matcherMyBool;
            }
        }
    }
}
