namespace Entitas {
    public partial class Entity {
        public MyStringComponent myString { get { return (MyStringComponent)GetComponent(ComponentIds.MyString); } }

        public bool hasMyString { get { return HasComponent(ComponentIds.MyString); } }

        public void AddMyString(MyStringComponent component) {
            AddComponent(ComponentIds.MyString, component);
        }

        public void AddMyString(string newMyString) {
            var component = new MyStringComponent();
            component.myString = newMyString;
            AddMyString(component);
        }

        public void ReplaceMyString(string newMyString) {
            MyStringComponent component;
            if (hasMyString) {
                WillRemoveComponent(ComponentIds.MyString);
                component = myString;
            } else {
                component = new MyStringComponent();
            }
            component.myString = newMyString;
            ReplaceComponent(ComponentIds.MyString, component);
        }

        public void RemoveMyString() {
            RemoveComponent(ComponentIds.MyString);
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
