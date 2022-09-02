using System;
using System.Linq;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

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

    public static class ShouldGenerateComponentIndexComponentDataExtension {

        public const string COMPONENT_GENERATE_INDEX = "Component.Generate.Index";

        public static bool ShouldGenerateIndex(this ComponentData data) {
            return (bool)data[COMPONENT_GENERATE_INDEX];
        }

        public static void ShouldGenerateIndex(this ComponentData data, bool generate) {
            data[COMPONENT_GENERATE_INDEX] = generate;
        }
    }
}
