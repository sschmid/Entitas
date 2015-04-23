namespace Entitas {
    public partial class Entity {
        public MyFloatComponent myFloat { get { return (MyFloatComponent)GetComponent(ComponentIds.MyFloat); } }

        public bool hasMyFloat { get { return HasComponent(ComponentIds.MyFloat); } }

        public void AddMyFloat(MyFloatComponent component) {
            AddComponent(ComponentIds.MyFloat, component);
        }

        public void AddMyFloat(float newMyFloat) {
            var component = new MyFloatComponent();
            component.myFloat = newMyFloat;
            AddMyFloat(component);
        }

        public void ReplaceMyFloat(float newMyFloat) {
            MyFloatComponent component;
            if (hasMyFloat) {
                WillRemoveComponent(ComponentIds.MyFloat);
                component = myFloat;
            } else {
                component = new MyFloatComponent();
            }
            component.myFloat = newMyFloat;
            ReplaceComponent(ComponentIds.MyFloat, component);
        }

        public void RemoveMyFloat() {
            RemoveComponent(ComponentIds.MyFloat);
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
