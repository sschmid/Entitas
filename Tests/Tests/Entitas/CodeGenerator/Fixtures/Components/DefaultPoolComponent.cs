using Entitas;
using Entitas.CodeGenerator;

[Pool]
public class DefaultPoolComponent : IComponent {
    public static ComponentInfo componentInfo {
        get {
            return TypeReflectionProvider.GetComponentInfos(new [] { typeof(DefaultPoolComponent) })[0];
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

