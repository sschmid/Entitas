using Entitas;

namespace Entitas {
    public partial class Entity {
        static readonly EComponent eComponent = new EComponent();

        public bool isE {
            get { return HasComponent(PoolCComponentIds.E); }
            set {
                if (value != isE) {
                    if (value) {
                        AddComponent(PoolCComponentIds.E, eComponent);
                    } else {
                        RemoveComponent(PoolCComponentIds.E);
                    }
                }
            }
        }

        public Entity IsE(bool value) {
            isE = value;
            return this;
        }
    }
}

    public partial class PoolCMatcher {
        static IMatcher _matcherE;

        public static IMatcher E {
            get {
                if (_matcherE == null) {
                    var matcher = (Matcher)Matcher.AllOf(PoolCComponentIds.E);
                    matcher.componentNames = PoolCComponentIds.componentNames;
                    _matcherE = matcher;
                }

                return _matcherE;
            }
        }
    }
