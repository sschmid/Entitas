namespace Entitas {
    public partial class Entity {
        public DateTimeComponent dateTime { get { return (DateTimeComponent)GetComponent(ComponentIds.DateTime); } }

        public bool hasDateTime { get { return HasComponent(ComponentIds.DateTime); } }

        public void AddDateTime(DateTimeComponent component) {
            AddComponent(ComponentIds.DateTime, component);
        }

        public void AddDateTime(System.DateTime newDate) {
            var component = new DateTimeComponent();
            component.date = newDate;
            AddDateTime(component);
        }

        public void ReplaceDateTime(System.DateTime newDate) {
            DateTimeComponent component;
            if (hasDateTime) {
                WillRemoveComponent(ComponentIds.DateTime);
                component = dateTime;
            } else {
                component = new DateTimeComponent();
            }
            component.date = newDate;
            ReplaceComponent(ComponentIds.DateTime, component);
        }

        public void RemoveDateTime() {
            RemoveComponent(ComponentIds.DateTime);
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherDateTime;

        public static AllOfMatcher DateTime {
            get {
                if (_matcherDateTime == null) {
                    _matcherDateTime = new Matcher(ComponentIds.DateTime);
                }

                return _matcherDateTime;
            }
        }
    }
}
