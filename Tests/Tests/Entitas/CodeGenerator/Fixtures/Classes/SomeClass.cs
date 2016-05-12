using System.Collections.Generic;
using Entitas;
using Entitas.CodeGenerator;
using Entitas.Serialization;

[Pool("SomePool"), Pool("SomeOtherPool")]
public class SomeClass {
    public static ComponentInfo componentInfo {
        get {
            return new ComponentInfo(
                "SomeClassComponent",
                new List<PublicMemberInfo> {
                    new PublicMemberInfo(typeof(SomeClass), "value")
                },
                new [] { "SomePool", "SomeOtherPool" },
                false,
                "is",
                true,
                true,
                true
            );
        }
    }

    public static string extensions =
        @"using Entitas;

public class SomeClassComponent : IComponent {
    public SomeClass value;
}

namespace Entitas {
    public partial class Entity {
        public SomeClassComponent someClass { get { return (SomeClassComponent)GetComponent(SomePoolComponentIds.SomeClass); } }

        public bool hasSomeClass { get { return HasComponent(SomePoolComponentIds.SomeClass); } }

        public Entity AddSomeClass(SomeClass newValue) {
            var component = CreateComponent<SomeClassComponent>(SomePoolComponentIds.SomeClass);
            component.value = newValue;
            return AddComponent(SomePoolComponentIds.SomeClass, component);
        }

        public Entity ReplaceSomeClass(SomeClass newValue) {
            var component = CreateComponent<SomeClassComponent>(SomePoolComponentIds.SomeClass);
            component.value = newValue;
            ReplaceComponent(SomePoolComponentIds.SomeClass, component);
            return this;
        }

        public Entity RemoveSomeClass() {
            return RemoveComponent(SomePoolComponentIds.SomeClass);
        }
    }
}

    public partial class SomePoolMatcher {
        static IMatcher _matcherSomeClass;

        public static IMatcher SomeClass {
            get {
                if (_matcherSomeClass == null) {
                    var matcher = (Matcher)Matcher.AllOf(SomePoolComponentIds.SomeClass);
                    matcher.componentNames = SomePoolComponentIds.componentNames;
                    _matcherSomeClass = matcher;
                }

                return _matcherSomeClass;
            }
        }
    }

    public partial class SomeOtherPoolMatcher {
        static IMatcher _matcherSomeClass;

        public static IMatcher SomeClass {
            get {
                if (_matcherSomeClass == null) {
                    var matcher = (Matcher)Matcher.AllOf(SomeOtherPoolComponentIds.SomeClass);
                    matcher.componentNames = SomeOtherPoolComponentIds.componentNames;
                    _matcherSomeClass = matcher;
                }

                return _matcherSomeClass;
            }
        }
    }
";
}

