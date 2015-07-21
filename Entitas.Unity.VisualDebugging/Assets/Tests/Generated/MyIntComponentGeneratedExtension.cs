namespace Entitas {
    public partial class Entity {
        public MyIntComponent myInt { get { return (MyIntComponent)GetComponent(ComponentIds.MyInt); } }

        public bool hasMyInt { get { return HasComponent(ComponentIds.MyInt); } }

        public Entity AddMyInt(MyIntComponent component) {
            return AddComponent(ComponentIds.MyInt, component);
        }

        public Entity AddMyInt(int newMyInt) {
            var component = new MyIntComponent();
            component.myInt = newMyInt;
            return AddMyInt(component);
        }

        public Entity ReplaceMyInt(int newMyInt) {
            MyIntComponent component;
            if (hasMyInt) {
                component = myInt;
            } else {
                component = new MyIntComponent();
            }
            component.myInt = newMyInt;
            return ReplaceComponent(ComponentIds.MyInt, component);
        }

        public Entity RemoveMyInt() {
            return RemoveComponent(ComponentIds.MyInt);
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
