using System.Collections.Generic;

namespace Entitas {
    public partial class Entity {
        public DateTimeComponent dateTime { get { return (DateTimeComponent)GetComponent(ComponentIds.DateTime); } }

        public bool hasDateTime { get { return HasComponent(ComponentIds.DateTime); } }

        static readonly Stack<DateTimeComponent> _dateTimeComponentPool = new Stack<DateTimeComponent>();

        public static void ClearDateTimeComponentPool() {
            _dateTimeComponentPool.Clear();
        }

        public Entity AddDateTime(System.DateTime newDate) {
            var component = _dateTimeComponentPool.Count > 0 ? _dateTimeComponentPool.Pop() : new DateTimeComponent();
            component.date = newDate;
            return AddComponent(ComponentIds.DateTime, component);
        }

        public Entity ReplaceDateTime(System.DateTime newDate) {
            var previousComponent = hasDateTime ? dateTime : null;
            var component = _dateTimeComponentPool.Count > 0 ? _dateTimeComponentPool.Pop() : new DateTimeComponent();
            component.date = newDate;
            ReplaceComponent(ComponentIds.DateTime, component);
            if (previousComponent != null) {
                _dateTimeComponentPool.Push(previousComponent);
            }
            return this;
        }

        public Entity RemoveDateTime() {
            var component = dateTime;
            RemoveComponent(ComponentIds.DateTime);
            _dateTimeComponentPool.Push(component);
            return this;
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
