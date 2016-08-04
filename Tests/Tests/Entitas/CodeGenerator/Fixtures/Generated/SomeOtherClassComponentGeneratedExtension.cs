//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGenerator.ComponentExtensionsGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Entitas;

public class SomeOtherClassComponent : IComponent {
    public SomeNamespace.SomeOtherClass value;
}

namespace Entitas {
    public interface ISomeOtherClassEntity : IEntity {
        SomeOtherClassComponent someOtherClass { get; }
        bool hasSomeOtherClass { get; }
        ISomeOtherClassEntity AddSomeOtherClass(SomeNamespace.SomeOtherClass newValue);
        ISomeOtherClassEntity ReplaceSomeOtherClass(SomeNamespace.SomeOtherClass newValue);
        ISomeOtherClassEntity RemoveSomeOtherClass();
    }

    public partial class Entity {
        public SomeOtherClassComponent someOtherClass { get { return (SomeOtherClassComponent)GetComponent(SomePoolComponentIds.SomeOtherClass); } }

        public bool hasSomeOtherClass { get { return HasComponent(SomePoolComponentIds.SomeOtherClass); } }

        public Entity AddSomeOtherClass(SomeNamespace.SomeOtherClass newValue) {
            var component = CreateComponent<SomeOtherClassComponent>(SomePoolComponentIds.SomeOtherClass);
            component.value = newValue;
            return (Entity)AddComponent(SomePoolComponentIds.SomeOtherClass, component);
        }

        public Entity ReplaceSomeOtherClass(SomeNamespace.SomeOtherClass newValue) {
            var component = CreateComponent<SomeOtherClassComponent>(SomePoolComponentIds.SomeOtherClass);
            component.value = newValue;
            ReplaceComponent(SomePoolComponentIds.SomeOtherClass, component);
            return this;
        }

        public Entity RemoveSomeOtherClass() {
            return (Entity)RemoveComponent(SomePoolComponentIds.SomeOtherClass);
        }
    }
}

    public partial class SomePoolMatcher {
        static IMatcher _matcherSomeOtherClass;

        public static IMatcher SomeOtherClass {
            get {
                if (_matcherSomeOtherClass == null) {
                    var matcher = (Matcher)Matcher.AllOf(SomePoolComponentIds.SomeOtherClass);
                    matcher.componentNames = SomePoolComponentIds.componentNames;
                    _matcherSomeOtherClass = matcher;
                }

                return _matcherSomeOtherClass;
            }
        }
    }
