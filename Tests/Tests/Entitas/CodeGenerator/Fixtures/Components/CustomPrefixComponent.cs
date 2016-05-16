using Entitas;
using Entitas.CodeGenerator;

[SingleEntity, CustomPrefix("My")]
public class CustomPrefixComponent : IComponent {
    public static ComponentInfo componentInfo { 
        get {
            return TypeReflectionProvider.GetComponentInfos(new [] { typeof(CustomPrefixComponent) })[0];
        }
    }

    public static string extensions =
        @"namespace Entitas {
    public partial class Entity {
        static readonly CustomPrefixComponent customPrefixComponent = new CustomPrefixComponent();

        public bool myCustomPrefix {
            get { return HasComponent(ComponentIds.CustomPrefix); }
            set {
                if (value != myCustomPrefix) {
                    if (value) {
                        AddComponent(ComponentIds.CustomPrefix, customPrefixComponent);
                    } else {
                        RemoveComponent(ComponentIds.CustomPrefix);
                    }
                }
            }
        }

        public Entity MyCustomPrefix(bool value) {
            myCustomPrefix = value;
            return this;
        }
    }

    public partial class Pool {
        public Entity customPrefixEntity { get { return GetGroup(Matcher.CustomPrefix).GetSingleEntity(); } }

        public bool myCustomPrefix {
            get { return customPrefixEntity != null; }
            set {
                var entity = customPrefixEntity;
                if (value != (entity != null)) {
                    if (value) {
                        CreateEntity().myCustomPrefix = true;
                    } else {
                        DestroyEntity(entity);
                    }
                }
            }
        }
    }

    public partial class Matcher {
        static IMatcher _matcherCustomPrefix;

        public static IMatcher CustomPrefix {
            get {
                if (_matcherCustomPrefix == null) {
                    var matcher = (Matcher)Matcher.AllOf(ComponentIds.CustomPrefix);
                    matcher.componentNames = ComponentIds.componentNames;
                    _matcherCustomPrefix = matcher;
                }

                return _matcherCustomPrefix;
            }
        }
    }
}
";
    }

