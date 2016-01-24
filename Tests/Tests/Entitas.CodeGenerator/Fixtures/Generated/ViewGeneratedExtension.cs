namespace Entitas {
    public partial class Entity {
        static readonly View viewComponent = new View();

        public bool isView {
            get { return HasComponent(ComponentIds.View); }
            set {
                if (value != isView) {
                    if (value) {
                        AddComponent(ComponentIds.View, viewComponent);
                    } else {
                        RemoveComponent(ComponentIds.View);
                    }
                }
            }
        }

        public Entity IsView(bool value) {
            isView = value;
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherView;

        public static IMatcher View {
            get {
                if (_matcherView == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.View);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherView = matcher;
                }

                return _matcherView;
            }
        }
    }
}
