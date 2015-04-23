namespace Entitas {
    public partial class Entity {
        public MyEnumComponent myEnum { get { return (MyEnumComponent)GetComponent(ComponentIds.MyEnum); } }

        public bool hasMyEnum { get { return HasComponent(ComponentIds.MyEnum); } }

        public void AddMyEnum(MyEnumComponent component) {
            AddComponent(ComponentIds.MyEnum, component);
        }

        public void AddMyEnum(MyEnumComponent.MyEnum newMyEnum) {
            var component = new MyEnumComponent();
            component.myEnum = newMyEnum;
            AddMyEnum(component);
        }

        public void ReplaceMyEnum(MyEnumComponent.MyEnum newMyEnum) {
            MyEnumComponent component;
            if (hasMyEnum) {
                WillRemoveComponent(ComponentIds.MyEnum);
                component = myEnum;
            } else {
                component = new MyEnumComponent();
            }
            component.myEnum = newMyEnum;
            ReplaceComponent(ComponentIds.MyEnum, component);
        }

        public void RemoveMyEnum() {
            RemoveComponent(ComponentIds.MyEnum);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherMyEnum;

        public static AllOfMatcher MyEnum {
            get {
                if (_matcherMyEnum == null) {
                    _matcherMyEnum = new Matcher(ComponentIds.MyEnum);
                }

                return _matcherMyEnum;
            }
        }
    }
}
