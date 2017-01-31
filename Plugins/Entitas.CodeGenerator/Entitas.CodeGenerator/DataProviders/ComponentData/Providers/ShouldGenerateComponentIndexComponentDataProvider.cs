using System;
using System.Linq;
using Entitas.CodeGenerator.Api;

namespace Entitas.CodeGenerator {

    public class ShouldGenerateComponentIndexComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            data.ShouldGenerateIndex(getGenerateIndex(type));
        }

        bool getGenerateIndex(Type type) {
            var attr = Attribute
                .GetCustomAttributes(type)
                .OfType<DontGenerateAttribute>()
                .SingleOrDefault();

            return attr == null || attr.generateIndex;
        }
    }

    public static class ShouldGenerateComponentIndexComponentDataProviderExtension {

        public const string COMPONENT_GENERATE_INDEX = "component_generateIndex";

        public static bool ShouldGenerateIndex(this ComponentData data) {
            return (bool)data[COMPONENT_GENERATE_INDEX];
        }

        public static void ShouldGenerateIndex(this ComponentData data, bool generate) {
            data[COMPONENT_GENERATE_INDEX] = generate;
        }
    }
}
