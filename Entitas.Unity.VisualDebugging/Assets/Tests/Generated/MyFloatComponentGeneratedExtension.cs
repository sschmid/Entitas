namespace Entitas {
    public partial class Entity {
        public MyFloatComponent myFloat { get { return (MyFloatComponent)GetComponent(ComponentIds.MyFloat); } }

        public bool hasMyFloat { get { return HasComponent(ComponentIds.MyFloat); } }

        public Entity AddMyFloat(MyFloatComponent component) {
            return AddComponent(ComponentIds.MyFloat, component);
        }

        public Entity AddMyFloat(float newMyFloat) {
            var component = new MyFloatComponent();
            component.myFloat = newMyFloat;
            return AddMyFloat(component);
        }

        public Entity ReplaceMyFloat(float newMyFloat) {
            MyFloatComponent component;
            if (hasMyFloat) {
                component = myFloat;
            } else {
                component = new MyFloatComponent();
            }
            component.myFloat = newMyFloat;
            return ReplaceComponent(ComponentIds.MyFloat, component);
        }

        public Entity RemoveMyFloat() {
            return RemoveComponent(ComponentIds.MyFloat);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherMyFloat;

        public static AllOfMatcher MyFloat {
            get {
                if (_matcherMyFloat == null) {
                    _matcherMyFloat = new Matcher(ComponentIds.MyFloat);
                }

                return _matcherMyFloat;
            }
        }
    }
}
