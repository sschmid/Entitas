namespace Entitas {
    public partial class Entity {
        public MyEnumComponent myEnum { get { return (MyEnumComponent)GetComponent(ComponentIds.MyEnum); } }

        public bool hasMyEnum { get { return HasComponent(ComponentIds.MyEnum); } }

        public Entity AddMyEnum(MyEnumComponent component) {
            return AddComponent(ComponentIds.MyEnum, component);
        }

        public Entity AddMyEnum(MyEnumComponent.MyEnum newMyEnum) {
            var component = new MyEnumComponent();
            component.myEnum = newMyEnum;
            return AddMyEnum(component);
        }

        public Entity ReplaceMyEnum(MyEnumComponent.MyEnum newMyEnum) {
            MyEnumComponent component;
            if (hasMyEnum) {
                WillRemoveComponent(ComponentIds.MyEnum);
                component = myEnum;
            } else {
                component = new MyEnumComponent();
            }
            component.myEnum = newMyEnum;
            return ReplaceComponent(ComponentIds.MyEnum, component);
        }

        public Entity RemoveMyEnum() {
            return RemoveComponent(ComponentIds.MyEnum);
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
