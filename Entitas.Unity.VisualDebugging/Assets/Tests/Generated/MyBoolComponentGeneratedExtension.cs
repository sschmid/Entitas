namespace Entitas {
    public partial class Entity {
        public MyBoolComponent myBool { get { return (MyBoolComponent)GetComponent(ComponentIds.MyBool); } }

        public bool hasMyBool { get { return HasComponent(ComponentIds.MyBool); } }

        public Entity AddMyBool(MyBoolComponent component) {
            return AddComponent(ComponentIds.MyBool, component);
        }

        public Entity AddMyBool(bool newMyBool) {
            var component = new MyBoolComponent();
            component.myBool = newMyBool;
            return AddMyBool(component);
        }

        public Entity ReplaceMyBool(bool newMyBool) {
            MyBoolComponent component;
            if (hasMyBool) {
                component = myBool;
            } else {
                component = new MyBoolComponent();
            }
            component.myBool = newMyBool;
            return ReplaceComponent(ComponentIds.MyBool, component);
        }

        public Entity RemoveMyBool() {
            return RemoveComponent(ComponentIds.MyBool);
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
