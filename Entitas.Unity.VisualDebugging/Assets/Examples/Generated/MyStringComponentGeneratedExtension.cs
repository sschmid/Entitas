namespace Entitas {
    public partial class Entity {
        public MyStringComponent myString { get { return (MyStringComponent)GetComponent(ComponentIds.MyString); } }

        public bool hasMyString { get { return HasComponent(ComponentIds.MyString); } }

        public Entity AddMyString(string newMyString) {
            var componentPool = GetComponentPool(ComponentIds.MyString);
            var component = (MyStringComponent)(componentPool.Count > 0 ? componentPool.Pop() : new MyStringComponent());
            component.myString = newMyString;
            return AddComponent(ComponentIds.MyString, component);
        }

        public Entity ReplaceMyString(string newMyString) {
            var componentPool = GetComponentPool(ComponentIds.MyString);
            var component = (MyStringComponent)(componentPool.Count > 0 ? componentPool.Pop() : new MyStringComponent());
            component.myString = newMyString;
            ReplaceComponent(ComponentIds.MyString, component);
            return this;
        }

        public Entity RemoveMyString() {
            return RemoveComponent(ComponentIds.MyString);;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherMyString;

        public static IMatcher MyString {
            get {
                if (_matcherMyString == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.MyString);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherMyString = matcher;
                }

                return _matcherMyString;
            }
        }
    }
}
