using System;
using System.Linq;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public class ShouldGenerateMethodsComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            var generate = !Attribute
                .GetCustomAttributes(type)
                .OfType<DontGenerateAttribute>()
                .Any();

            data.ShouldGenerateMethods(generate);
        }
    }

    public static class ShouldGenerateMethodsComponentDataExtension {

        public const string COMPONENT_GENERATE_METHODS = "Component.Generate.Methods";

        public static bool ShouldGenerateMethods(this ComponentData data) {
            return (bool)data[COMPONENT_GENERATE_METHODS];
        }

        public static void ShouldGenerateMethods(this ComponentData data, bool generate) {
            data[COMPONENT_GENERATE_METHODS] = generate;
        }
    }
}
