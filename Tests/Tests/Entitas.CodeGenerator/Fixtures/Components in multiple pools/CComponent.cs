using Entitas.CodeGenerator;
using Entitas;

[Pool("PoolA"), Pool("PoolB"), Pool("PoolC")]
public class CComponent : IComponent {
    public static string extensions =
        @"using Entitas;

namespace Entitas {
    public partial class Entity {
        static readonly CComponent cComponent = new CComponent();

        public bool isC {
            get { return HasComponent(PoolAComponentIds.C); }
            set {
                if (value != isC) {
                    if (value) {
                        AddComponent(PoolAComponentIds.C, cComponent);
                    } else {
                        RemoveComponent(PoolAComponentIds.C);
                    }
                }
            }
        }

        public Entity IsC(bool value) {
            isC = value;
            return this;
        }
    }
}

    public partial class PoolAMatcher {
        static IMatcher _matcherC;

        public static IMatcher C {
            get {
                if (_matcherC == null) {
                    _matcherC = Matcher.AllOf(PoolAComponentIds.C);
                }

                return _matcherC;
            }
        }
    }

    public partial class PoolBMatcher {
        static IMatcher _matcherC;

        public static IMatcher C {
            get {
                if (_matcherC == null) {
                    _matcherC = Matcher.AllOf(PoolAComponentIds.C);
                }

                return _matcherC;
            }
        }
    }

    public partial class PoolCMatcher {
        static IMatcher _matcherC;

        public static IMatcher C {
            get {
                if (_matcherC == null) {
                    _matcherC = Matcher.AllOf(PoolAComponentIds.C);
                }

                return _matcherC;
            }
        }
    }
";
}

