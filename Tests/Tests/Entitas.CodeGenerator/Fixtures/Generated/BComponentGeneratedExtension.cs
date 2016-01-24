using Entitas;

namespace Entitas {
    public partial class Entity {
        static readonly BComponent bComponent = new BComponent();

        public bool isB {
            get { return HasComponent(PoolAComponentIds.B); }
            set {
                if (value != isB) {
                    if (value) {
                        AddComponent(PoolAComponentIds.B, bComponent);
                    } else {
                        RemoveComponent(PoolAComponentIds.B);
                    }
                }
            }
        }

        public Entity IsB(bool value) {
            isB = value;
            return this;
        }
    }
}

    public partial class PoolAMatcher {
        static IMatcher _matcherB;

        public static IMatcher B {
            get {
                if (_matcherB == null) {
                    var matcher = (Matcher)Matcher.AllOf(PoolAComponentIds.B);
                    matcher.componentNames = PoolAComponentIds.componentNames;
                    _matcherB = matcher;
                }

                return _matcherB;
            }
        }
    }

    public partial class PoolBMatcher {
        static IMatcher _matcherB;

        public static IMatcher B {
            get {
                if (_matcherB == null) {
                    var matcher = (Matcher)Matcher.AllOf(PoolAComponentIds.B);
                    matcher.componentNames = PoolAComponentIds.componentNames;
                    _matcherB = matcher;
                }

                return _matcherB;
            }
        }
    }
