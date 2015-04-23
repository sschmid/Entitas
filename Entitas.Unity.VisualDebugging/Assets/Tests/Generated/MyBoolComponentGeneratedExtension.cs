namespace Entitas {
    public partial class Entity {
        public MyBoolComponent myBool { get { return (MyBoolComponent)GetComponent(ComponentIds.MyBool); } }

        public bool hasMyBool { get { return HasComponent(ComponentIds.MyBool); } }

        public void AddMyBool(MyBoolComponent component) {
            AddComponent(ComponentIds.MyBool, component);
        }

        public void AddMyBool(bool newMyBool) {
            var component = new MyBoolComponent();
            component.myBool = newMyBool;
            AddMyBool(component);
        }

        public void ReplaceMyBool(bool newMyBool) {
            MyBoolComponent component;
            if (hasMyBool) {
                WillRemoveComponent(ComponentIds.MyBool);
                component = myBool;
            } else {
                component = new MyBoolComponent();
            }
            component.myBool = newMyBool;
            ReplaceComponent(ComponentIds.MyBool, component);
        }

        public void RemoveMyBool() {
            RemoveComponent(ComponentIds.MyBool);
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
