namespace Entitas {
    public partial class Entity {
        public MonoBehaviourSubClassComponent monoBehaviourSubClass { get { return (MonoBehaviourSubClassComponent)GetComponent(ComponentIds.MonoBehaviourSubClass); } }

        public bool hasMonoBehaviourSubClass { get { return HasComponent(ComponentIds.MonoBehaviourSubClass); } }

        public Entity AddMonoBehaviourSubClass(MonoBehaviourSubClass newMonoBehaviour) {
            var componentPool = GetComponentPool(ComponentIds.MonoBehaviourSubClass);
            var component = (MonoBehaviourSubClassComponent)(componentPool.Count > 0 ? componentPool.Pop() : new MonoBehaviourSubClassComponent());
            component.monoBehaviour = newMonoBehaviour;
            return AddComponent(ComponentIds.MonoBehaviourSubClass, component);
        }

        public Entity ReplaceMonoBehaviourSubClass(MonoBehaviourSubClass newMonoBehaviour) {
            var componentPool = GetComponentPool(ComponentIds.MonoBehaviourSubClass);
            var component = (MonoBehaviourSubClassComponent)(componentPool.Count > 0 ? componentPool.Pop() : new MonoBehaviourSubClassComponent());
            component.monoBehaviour = newMonoBehaviour;
            ReplaceComponent(ComponentIds.MonoBehaviourSubClass, component);
            return this;
        }

        public Entity RemoveMonoBehaviourSubClass() {
            return RemoveComponent(ComponentIds.MonoBehaviourSubClass);;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherMonoBehaviourSubClass;

        public static IMatcher MonoBehaviourSubClass {
            get {
                if (_matcherMonoBehaviourSubClass == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.MonoBehaviourSubClass);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherMonoBehaviourSubClass = matcher;
                }

                return _matcherMonoBehaviourSubClass;
            }
        }
    }
}
