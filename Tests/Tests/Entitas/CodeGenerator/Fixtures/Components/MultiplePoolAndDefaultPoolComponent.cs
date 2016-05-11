using Entitas;
using Entitas.CodeGenerator;
using Entitas.Serialization;

public class MultiplePoolAndDefaultPoolComponent : IComponent {
    public static ComponentInfo componentInfo {
        get {
            return new ComponentInfo(
                typeof(MultiplePoolAndDefaultPoolComponent).ToCompilableString(),
                new System.Collections.Generic.List<PublicMemberInfo>(),
                new [] { "", "Other" },
                false,
                "is",
                false,
                true,
                true
            );
        }
    }

    public static string extensions = @"using Entitas;

namespace Entitas {
    public partial class Entity {
        static readonly MultiplePoolAndDefaultPoolComponent multiplePoolAndDefaultPoolComponent = new MultiplePoolAndDefaultPoolComponent();

        public bool isMultiplePoolAndDefaultPool {
            get { return HasComponent(ComponentIds.MultiplePoolAndDefaultPool); }
            set {
                if (value != isMultiplePoolAndDefaultPool) {
                    if (value) {
                        AddComponent(ComponentIds.MultiplePoolAndDefaultPool, multiplePoolAndDefaultPoolComponent);
                    } else {
                        RemoveComponent(ComponentIds.MultiplePoolAndDefaultPool);
                    }
                }
            }
        }

        public Entity IsMultiplePoolAndDefaultPool(bool value) {
            isMultiplePoolAndDefaultPool = value;
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherMultiplePoolAndDefaultPool;

        public static IMatcher MultiplePoolAndDefaultPool {
            get {
                if (_matcherMultiplePoolAndDefaultPool == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.MultiplePoolAndDefaultPool);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherMultiplePoolAndDefaultPool = matcher;
                }

                return _matcherMultiplePoolAndDefaultPool;
            }
        }
    }
}

    public partial class OtherMatcher {
        static IMatcher _matcherMultiplePoolAndDefaultPool;

        public static IMatcher MultiplePoolAndDefaultPool {
            get {
                if (_matcherMultiplePoolAndDefaultPool == null) {
                    var matcher = (Matcher)Matcher.AllOf(OtherComponentIds.MultiplePoolAndDefaultPool);
                    matcher.componentNames = OtherComponentIds.componentNames;
                    _matcherMultiplePoolAndDefaultPool = matcher;
                }

                return _matcherMultiplePoolAndDefaultPool;
            }
        }
    }
";
}

