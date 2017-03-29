using System;

namespace Entitas.CodeGenerator {

    public class ComponentDataProvider : AbstractComponentDataProvider {

        public override string name { get { return "Component"; } }
        public override int priority { get { return 0; } }
        public override bool isEnabledByDefault { get { return true; } }
        public override bool runInDryMode { get { return true; } }

        public ComponentDataProvider() : this(null) {
        }

        public ComponentDataProvider(Type[] types)
            : base(types, getComponentDataProviders()) {
        }

        static IComponentDataProvider[] getComponentDataProviders() {
            return new IComponentDataProvider[] {
                new ComponentTypeComponentDataProvider(),
                new MemberDataComponentDataProvider(),
                new ContextsComponentDataProvider(),
                new IsUniqueComponentDataProvider(),
                new UniquePrefixComponentDataProvider(),
                new ShouldGenerateComponentComponentDataProvider(),
                new ShouldGenerateMethodsComponentDataProvider(),
                new ShouldGenerateComponentIndexComponentDataProvider()
            };
        }
    }
}
