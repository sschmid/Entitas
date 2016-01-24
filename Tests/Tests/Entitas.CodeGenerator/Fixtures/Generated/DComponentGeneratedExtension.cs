using Entitas;

namespace Entitas {
    public partial class Entity {
        static readonly DComponent dComponent = new DComponent();

        public bool isD {
            get { return HasComponent(PoolBComponentIds.D); }
            set {
                if (value != isD) {
                    if (value) {
                        AddComponent(PoolBComponentIds.D, dComponent);
                    } else {
                        RemoveComponent(PoolBComponentIds.D);
                    }
                }
            }
        }

        public Entity IsD(bool value) {
            isD = value;
            return this;
        }
    }
}

    public partial class PoolBMatcher {
        static IMatcher _matcherD;

        public static IMatcher D {
            get {
                if (_matcherD == null) {
                    var matcher = (Matcher)Matcher.AllOf(PoolBComponentIds.D);
                    matcher.componentNames = PoolBComponentIds.componentNames;
                    _matcherD = matcher;
                }

                return _matcherD;
            }
        }
    }

    public partial class PoolCMatcher {
        static IMatcher _matcherD;

        public static IMatcher D {
            get {
                if (_matcherD == null) {
                    var matcher = (Matcher)Matcher.AllOf(PoolBComponentIds.D);
                    matcher.componentNames = PoolBComponentIds.componentNames;
                    _matcherD = matcher;
                }

                return _matcherD;
            }
        }
    }
