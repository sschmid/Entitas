namespace Entitas {
    public partial class Entity {
        public MyStringComponent myString { get { return (MyStringComponent)GetComponent(ComponentIds.MyString); } }

        public bool hasMyString { get { return HasComponent(ComponentIds.MyString); } }

        public Entity AddMyString(MyStringComponent component) {
            return AddComponent(ComponentIds.MyString, component);
        }

        public Entity AddMyString(string newMyString) {
            var component = new MyStringComponent();
            component.myString = newMyString;
            return AddMyString(component);
        }

        public Entity ReplaceMyString(string newMyString) {
            MyStringComponent component;
            if (hasMyString) {
                WillRemoveComponent(ComponentIds.MyString);
                component = myString;
            } else {
                component = new MyStringComponent();
            }
            component.myString = newMyString;
            return ReplaceComponent(ComponentIds.MyString, component);
        }

        public Entity RemoveMyString() {
            return RemoveComponent(ComponentIds.MyString);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherMyString;

        public static AllOfMatcher MyString {
            get {
                if (_matcherMyString == null) {
                    _matcherMyString = new Matcher(ComponentIds.MyString);
                }

                return _matcherMyString;
            }
        }
    }
}
