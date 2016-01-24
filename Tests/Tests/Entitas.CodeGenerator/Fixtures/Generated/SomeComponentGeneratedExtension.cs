namespace Entitas {
    public partial class Entity {
        static readonly SomeComponent someComponent = new SomeComponent();

        public bool isSome {
            get { return HasComponent(ComponentIds.Some); }
            set {
                if (value != isSome) {
                    if (value) {
                        AddComponent(ComponentIds.Some, someComponent);
                    } else {
                        RemoveComponent(ComponentIds.Some);
                    }
                }
            }
        }

        public Entity IsSome(bool value) {
            isSome = value;
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherSome;

        public static IMatcher Some {
            get {
                if (_matcherSome == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.Some);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherSome = matcher;
                }

                return _matcherSome;
            }
        }
    }
}
