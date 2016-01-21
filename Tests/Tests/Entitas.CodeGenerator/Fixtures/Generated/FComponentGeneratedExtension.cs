using Entitas;

namespace Entitas {
    public partial class Entity {
        static readonly FComponent fComponent = new FComponent();

        public bool isF {
            get { return HasComponent(PoolCComponentIds.F); }
            set {
                if (value != isF) {
                    if (value) {
                        AddComponent(PoolCComponentIds.F, fComponent);
                    } else {
                        RemoveComponent(PoolCComponentIds.F);
                    }
                }
            }
        }

        public Entity IsF(bool value) {
            isF = value;
            return this;
        }
    }
}

    public partial class PoolCMatcher {
        static IMatcher _matcherF;

        public static IMatcher F {
            get {
                if (_matcherF == null) {
                    var matcher = (Matcher)Matcher.AllOf(PoolCComponentIds.F);
                    matcher.componentNames = PoolCComponentIds.componentNames;
                    _matcherF = matcher;
                }

                return _matcherF;
            }
        }
    }
