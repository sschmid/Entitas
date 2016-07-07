using Entitas;
using Entitas.CodeGenerator;
using Entitas.Serialization;

namespace My.Namespace {
    public class NamespaceComponent : IComponent {
        public static ComponentInfo componentInfo { 
            get {
                return new ComponentInfo(
                    typeof(NamespaceComponent).ToCompilableString(),
                    new System.Collections.Generic.List<PublicMemberInfo>(),
                    new string[0],
                    false,
                    "is",
                    false,
                    true,
                    true
                );
            }
        }

        public static string extensions =
            @"namespace Entitas {
    public partial class Entity {
        static readonly My.Namespace.NamespaceComponent namespaceComponent = new My.Namespace.NamespaceComponent();

        public bool isNamespace {
            get { return HasComponent(ComponentIds.Namespace); }
            set {
                if (value != isNamespace) {
                    if (value) {
                        AddComponent(ComponentIds.Namespace, namespaceComponent);
                    } else {
                        RemoveComponent(ComponentIds.Namespace);
                    }
                }
            }
        }

        public Entity IsNamespace(bool value) {
            isNamespace = value;
            return this;
        }
    }

    public partial class Matcher {
        static IMatcher _matcherNamespace;

        public static IMatcher Namespace {
            get {
                if (_matcherNamespace == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.Namespace);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherNamespace = matcher;
                }

                return _matcherNamespace;
            }
        }
    }
}
";

    }
}

