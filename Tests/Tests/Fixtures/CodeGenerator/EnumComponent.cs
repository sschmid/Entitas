using Entitas;

public class MyEnumComponent : IComponent {
    public enum TheEnum {
        A,
        B
    }

    public TheEnum theEnum;
    public static string extensions = @"namespace Entitas {
    public partial class Entity {
        public MyEnumComponent myEnum { get { return (MyEnumComponent)GetComponent(ComponentIds.MyEnum); } }

        public bool hasMyEnum { get { return HasComponent(ComponentIds.MyEnum); } }

        public void AddMyEnum(MyEnumComponent component) {
            AddComponent(ComponentIds.MyEnum, component);
        }

        public void AddMyEnum(MyEnumComponent.TheEnum newTheEnum) {
            var component = new MyEnumComponent();
            component.theEnum = newTheEnum;
            AddMyEnum(component);
        }

        public void ReplaceMyEnum(MyEnumComponent.TheEnum newTheEnum) {
            MyEnumComponent component;
            if (hasMyEnum) {
                WillRemoveComponent(ComponentIds.MyEnum);
                component = myEnum;
            } else {
                component = new MyEnumComponent();
            }
            component.theEnum = newTheEnum;
            ReplaceComponent(ComponentIds.MyEnum, component);
        }

        public void RemoveMyEnum() {
            RemoveComponent(ComponentIds.MyEnum);
        }
    }

    public static partial class Matcher {
        static AllOfMatcher _matcherMyEnum;

        public static AllOfMatcher MyEnum {
            get {
                if (_matcherMyEnum == null) {
                    _matcherMyEnum = Matcher.AllOf(new [] { ComponentIds.MyEnum });
                }

                return _matcherMyEnum;
            }
        }
    }
}";
}

