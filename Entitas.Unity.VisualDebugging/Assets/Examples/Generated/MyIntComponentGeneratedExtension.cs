namespace Entitas {
    public partial class Entity {
        public MyIntComponent myInt { get { return (MyIntComponent)GetComponent(ComponentIds.MyInt); } }

        public bool hasMyInt { get { return HasComponent(ComponentIds.MyInt); } }

        public Entity AddMyInt(int newMyInt) {
            var componentPool = GetComponentPool(ComponentIds.MyInt);
            var component = (MyIntComponent)(componentPool.Count > 0 ? componentPool.Pop() : new MyIntComponent());
            component.myInt = newMyInt;
            return AddComponent(ComponentIds.MyInt, component);
        }

        public Entity ReplaceMyInt(int newMyInt) {
            var componentPool = GetComponentPool(ComponentIds.MyInt);
            var component = (MyIntComponent)(componentPool.Count > 0 ? componentPool.Pop() : new MyIntComponent());
            component.myInt = newMyInt;
            ReplaceComponent(ComponentIds.MyInt, component);
            return this;
        }

        public Entity RemoveMyInt() {
            return RemoveComponent(ComponentIds.MyInt);;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherMyInt;

        public static IMatcher MyInt {
            get {
                if (_matcherMyInt == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.MyInt);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherMyInt = matcher;
                }

                return _matcherMyInt;
            }
        }
    }
}
