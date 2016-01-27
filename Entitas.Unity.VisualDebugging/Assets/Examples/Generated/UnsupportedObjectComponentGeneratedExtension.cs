namespace Entitas {
    public partial class Entity {
        public UnsupportedObjectComponent unsupportedObject { get { return (UnsupportedObjectComponent)GetComponent(ComponentIds.UnsupportedObject); } }

        public bool hasUnsupportedObject { get { return HasComponent(ComponentIds.UnsupportedObject); } }

        public Entity AddUnsupportedObject(UnsupportedObject newUnsupportedObject) {
            var componentPool = GetComponentPool(ComponentIds.UnsupportedObject);
            var component = (UnsupportedObjectComponent)(componentPool.Count > 0 ? componentPool.Pop() : new UnsupportedObjectComponent());
            component.unsupportedObject = newUnsupportedObject;
            return AddComponent(ComponentIds.UnsupportedObject, component);
        }

        public Entity ReplaceUnsupportedObject(UnsupportedObject newUnsupportedObject) {
            var componentPool = GetComponentPool(ComponentIds.UnsupportedObject);
            var component = (UnsupportedObjectComponent)(componentPool.Count > 0 ? componentPool.Pop() : new UnsupportedObjectComponent());
            component.unsupportedObject = newUnsupportedObject;
            ReplaceComponent(ComponentIds.UnsupportedObject, component);
            return this;
        }

        public Entity RemoveUnsupportedObject() {
            return RemoveComponent(ComponentIds.UnsupportedObject);;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherUnsupportedObject;

        public static IMatcher UnsupportedObject {
            get {
                if (_matcherUnsupportedObject == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.UnsupportedObject);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherUnsupportedObject = matcher;
                }

                return _matcherUnsupportedObject;
            }
        }
    }
}
