namespace Entitas {
    public partial class Entity {
        public MyFloatComponent myFloat { get { return (MyFloatComponent)GetComponent(ComponentIds.MyFloat); } }

        public bool hasMyFloat { get { return HasComponent(ComponentIds.MyFloat); } }

        public Entity AddMyFloat(float newMyFloat) {
            var componentPool = GetComponentPool(ComponentIds.MyFloat);
            var component = (MyFloatComponent)(componentPool.Count > 0 ? componentPool.Pop() : new MyFloatComponent());
            component.myFloat = newMyFloat;
            return AddComponent(ComponentIds.MyFloat, component);
        }

        public Entity ReplaceMyFloat(float newMyFloat) {
            var componentPool = GetComponentPool(ComponentIds.MyFloat);
            var component = (MyFloatComponent)(componentPool.Count > 0 ? componentPool.Pop() : new MyFloatComponent());
            component.myFloat = newMyFloat;
            ReplaceComponent(ComponentIds.MyFloat, component);
            return this;
        }

        public Entity RemoveMyFloat() {
            return RemoveComponent(ComponentIds.MyFloat);;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherMyFloat;

        public static IMatcher MyFloat {
            get {
                if (_matcherMyFloat == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.MyFloat);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherMyFloat = matcher;
                }

                return _matcherMyFloat;
            }
        }
    }
}
