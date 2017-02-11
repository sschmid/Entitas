using System;
using System.Reflection;

namespace Entitas.CodeGenerator {

    public class NamespaceComponentDataProvider : AbstractComponentDataProvider {

        public override string name { get { return "Component (with namespace)"; } }
        public override bool isEnabledByDefault { get { return false; } }

        public NamespaceComponentDataProvider() : this(Assembly.GetAssembly(typeof(IEntity)).GetTypes()) {
        }

        public NamespaceComponentDataProvider(Type[] types)
            : base(new IComponentDataProvider[] {
                new ComponentTypeComponentDataProvider(),
                new NamespaceComponentNameComponentDataProvider(),
                new MemberInfosComponentDataProvider(),
                new ContextsComponentDataProvider(),
                new IsUniqueComponentDataProvider(),
                new UniquePrefixComponentDataProvider(),
                new ShouldGenerateComponentComponentDataProvider(),
                new ShouldGenerateMethodsComponentDataProvider(),
                new ShouldGenerateComponentIndexComponentDataProvider(),
                new ShouldHideInBlueprintInspectorComponentDataProvider()
            }, types) {
        }
    }
}
