namespace Entitas {
    public partial class Entity {
        public MyIntComponent myInt { get { return (MyIntComponent)GetComponent(ComponentIds.MyInt); } }

        public bool hasMyInt { get { return HasComponent(ComponentIds.MyInt); } }

        public void AddMyInt(MyIntComponent component) {
            AddComponent(ComponentIds.MyInt, component);
        }

        public void AddMyInt(int newMyInt) {
            var component = new MyIntComponent();
            component.myInt = newMyInt;
            AddMyInt(component);
        }

        public void ReplaceMyInt(int newMyInt) {
            MyIntComponent component;
            if (hasMyInt) {
                WillRemoveComponent(ComponentIds.MyInt);
                component = myInt;
            } else {
                component = new MyIntComponent();
            }
            component.myInt = newMyInt;
            ReplaceComponent(ComponentIds.MyInt, component);
        }

        public void RemoveMyInt() {
            RemoveComponent(ComponentIds.MyInt);
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
