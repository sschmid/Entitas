using Entitas;

namespace Entitas {
    public partial class Entity {
        static readonly AComponent aComponent = new AComponent();

        public bool isA {
            get { return HasComponent(PoolAComponentIds.A); }
            set {
                if (value != isA) {
                    if (value) {
                        AddComponent(PoolAComponentIds.A, aComponent);
                    } else {
                        RemoveComponent(PoolAComponentIds.A);
                    }
                }
            }
        }

        public Entity IsA(bool value) {
            isA = value;
            return this;
        }
    }
}

    public partial class PoolAMatcher {
        static IMatcher _matcherA;

        public static IMatcher A {
            get {
                if (_matcherA == null) {
                    var matcher = (Matcher)Matcher.AllOf(PoolAComponentIds.A);
                    matcher.componentNames = PoolAComponentIds.componentNames;
                    _matcherA = matcher;
                }

                return _matcherA;
            }
        }
    }
