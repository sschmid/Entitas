using Entitas;
using Entitas.CodeGenerator;
using Entitas.Serialization;

public class DefaultPoolComponent : IComponent {
    public static ComponentInfo componentInfo {
        get {
            return new ComponentInfo(
                typeof(DefaultPoolComponent).ToCompilableString(),
                new System.Collections.Generic.List<PublicMemberInfo>(),
                new [] { "" },
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
        static readonly DefaultPoolComponent defaultPoolComponent = new DefaultPoolComponent();

        public bool isDefaultPool {
            get { return HasComponent(ComponentIds.DefaultPool); }
            set {
                if (value != isDefaultPool) {
                    if (value) {
                        AddComponent(ComponentIds.DefaultPool, defaultPoolComponent);
                    } else {
                        RemoveComponent(ComponentIds.DefaultPool);
                    }
                }
            }
        }

        public Entity IsDefaultPool(bool value) {
            isDefaultPool = value;
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherDefaultPool;

        public static IMatcher DefaultPool {
            get {
                if (_matcherDefaultPool == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.DefaultPool);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherDefaultPool = matcher;
                }

                return _matcherDefaultPool;
            }
        }
    }
}
";
}

