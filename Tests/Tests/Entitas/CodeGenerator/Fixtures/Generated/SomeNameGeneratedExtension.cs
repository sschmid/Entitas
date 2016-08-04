//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGenerator.ComponentExtensionsGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Entitas;

public class SomeName : IComponent {
    public CustomNames value;
}

namespace Entitas {
    public interface ISomeNameEntity : IEntity {
        SomeName someName { get; }
        bool hasSomeName { get; }
        ISomeNameEntity AddSomeName(CustomNames newValue);
        ISomeNameEntity ReplaceSomeName(CustomNames newValue);
        ISomeNameEntity RemoveSomeName();
    }

    public partial class Entity {
        public SomeName someName { get { return (SomeName)GetComponent(SomePoolComponentIds.SomeName); } }

        public bool hasSomeName { get { return HasComponent(SomePoolComponentIds.SomeName); } }

        public Entity AddSomeName(CustomNames newValue) {
            var component = CreateComponent<SomeName>(SomePoolComponentIds.SomeName);
            component.value = newValue;
            return (Entity)AddComponent(SomePoolComponentIds.SomeName, component);
        }

        public Entity ReplaceSomeName(CustomNames newValue) {
            var component = CreateComponent<SomeName>(SomePoolComponentIds.SomeName);
            component.value = newValue;
            ReplaceComponent(SomePoolComponentIds.SomeName, component);
            return this;
        }

        public Entity RemoveSomeName() {
            return (Entity)RemoveComponent(SomePoolComponentIds.SomeName);
        }
    }
}

    public partial class SomePoolMatcher {
        static IMatcher _matcherSomeName;

        public static IMatcher SomeName {
            get {
                if (_matcherSomeName == null) {
                    var matcher = (Matcher)Matcher.AllOf(SomePoolComponentIds.SomeName);
                    matcher.componentNames = SomePoolComponentIds.componentNames;
                    _matcherSomeName = matcher;
                }

                return _matcherSomeName;
            }
        }
    }
