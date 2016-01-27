namespace Entitas {
    public partial class Entity {
        public DateTimeComponent dateTime { get { return (DateTimeComponent)GetComponent(ComponentIds.DateTime); } }

        public bool hasDateTime { get { return HasComponent(ComponentIds.DateTime); } }

        public Entity AddDateTime(System.DateTime newDate) {
            var componentPool = GetComponentPool(ComponentIds.DateTime);
            var component = (DateTimeComponent)(componentPool.Count > 0 ? componentPool.Pop() : new DateTimeComponent());
            component.date = newDate;
            return AddComponent(ComponentIds.DateTime, component);
        }

        public Entity ReplaceDateTime(System.DateTime newDate) {
            var componentPool = GetComponentPool(ComponentIds.DateTime);
            var component = (DateTimeComponent)(componentPool.Count > 0 ? componentPool.Pop() : new DateTimeComponent());
            component.date = newDate;
            ReplaceComponent(ComponentIds.DateTime, component);
            return this;
        }

        public Entity RemoveDateTime() {
            return RemoveComponent(ComponentIds.DateTime);;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherDateTime;

        public static IMatcher DateTime {
            get {
                if (_matcherDateTime == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.DateTime);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherDateTime = matcher;
                }

                return _matcherDateTime;
            }
        }
    }
}
