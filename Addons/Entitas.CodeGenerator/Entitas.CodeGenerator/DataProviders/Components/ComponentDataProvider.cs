using System;
using System.Reflection;

namespace Entitas.CodeGenerator {

    public class ComponentDataProvider : AbstractComponentDataProvider {

        public override string name { get { return "Component"; } }
        public override bool isEnabledByDefault { get { return true; } }

        public ComponentDataProvider()
            : this(Assembly.LoadFrom(new CodeGeneratorConfig(EntitasPreferences.LoadConfig()).assemblyPath).GetTypes()) {
        }
        
        public ComponentDataProvider(Type[] types)
            : this(types, new CodeGeneratorConfig(EntitasPreferences.LoadConfig()).contexts[0]) {
        }

        public ComponentDataProvider(Type[] types, string defaultContextName)
            : base(getComponentDataProviders(defaultContextName), types) {
        }

        static IComponentDataProvider[] getComponentDataProviders(string defaultContextName) {
            return new IComponentDataProvider[] {
                new ComponentTypeComponentDataProvider(),
                new MemberDataComponentDataProvider(),
                new ContextsComponentDataProvider(defaultContextName),
                new IsUniqueComponentDataProvider(),
                new UniquePrefixComponentDataProvider(),
                new ShouldGenerateComponentComponentDataProvider(),
                new ShouldGenerateMethodsComponentDataProvider(),
                new ShouldGenerateComponentIndexComponentDataProvider()
            };
        }
    }
}
