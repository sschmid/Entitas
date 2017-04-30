using System;
using System.Linq;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public class CustomPrefixComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            data.SetCustomComponentPrefix(getUniqueComponentPrefix(type));
        }

        string getUniqueComponentPrefix(Type type) {
            var attr = Attribute.GetCustomAttributes(type)
                                .OfType<CustomPrefixAttribute>()
                                .SingleOrDefault();

            return attr == null ? "is" : attr.prefix;
        }
    }

    public static class UniquePrefixComponentDataExtension {

        public const string COMPONENT_CUSTOM_PREFIX = "component_customPrefix";

        public static string GetCustomComponentPrefix(this ComponentData data) {
            return (string)data[COMPONENT_CUSTOM_PREFIX];
        }

        public static void SetCustomComponentPrefix(this ComponentData data, string prefix) {
            data[COMPONENT_CUSTOM_PREFIX] = prefix;
        }
    }
}
