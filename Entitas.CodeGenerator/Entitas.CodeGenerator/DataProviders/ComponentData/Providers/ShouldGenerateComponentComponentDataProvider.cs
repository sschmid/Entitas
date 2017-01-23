using System;

namespace Entitas.CodeGenerator {

    public class ShouldGenerateComponentComponentDataProvider : IComponentDataProvider {

        readonly bool _shouldGenerateComponent;

        public ShouldGenerateComponentComponentDataProvider(bool shouldGenerateComponent) {
            _shouldGenerateComponent = shouldGenerateComponent;
        }

        public void Provide(Type type, ComponentData data) {
            data.ShouldGenerateComponent(_shouldGenerateComponent);
        }
    }

    public static class ShouldGenerateComponentComponentDataProviderExtension {

        public const string COMPONENT_GENERATE_COMPONENT = "component_generateComponent";

        public static bool ShouldGenerateComponent(this ComponentData data) {
            return (bool)data[COMPONENT_GENERATE_COMPONENT];
        }

        public static void ShouldGenerateComponent(this ComponentData data, bool generate) {
            data[COMPONENT_GENERATE_COMPONENT] = generate;
        }
    }
}
