using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public MyEnumComponent myEnum { get { return (MyEnumComponent)GetComponent(ComponentIds.MyEnum); } }

        public bool hasMyEnum { get { return HasComponent(ComponentIds.MyEnum); } }

        static readonly Stack<MyEnumComponent> _myEnumComponentPool = new Stack<MyEnumComponent>();

        public static void ClearMyEnumComponentPool() {
            _myEnumComponentPool.Clear();
        }

        public Entity AddMyEnum(MyEnumComponent.MyEnum newMyEnum) {
            var component = _myEnumComponentPool.Count > 0 ? _myEnumComponentPool.Pop() : new MyEnumComponent();
            component.myEnum = newMyEnum;
            return AddComponent(ComponentIds.MyEnum, component);
        }

        public Entity ReplaceMyEnum(MyEnumComponent.MyEnum newMyEnum) {
            var previousComponent = hasMyEnum ? myEnum : null;
            var component = _myEnumComponentPool.Count > 0 ? _myEnumComponentPool.Pop() : new MyEnumComponent();
            component.myEnum = newMyEnum;
            ReplaceComponent(ComponentIds.MyEnum, component);
            if (previousComponent != null) {
                _myEnumComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveMyEnum() {
            var component = myEnum;
            RemoveComponent(ComponentIds.MyEnum);
            _myEnumComponentPool.Push(component);
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherMyEnum;

        public static IMatcher MyEnum {
            get {
                if (_matcherMyEnum == null) {
                    _matcherMyEnum = Matcher.AllOf(ComponentIds.MyEnum);
                }

                return _matcherMyEnum;
            }
        }
    }
}
