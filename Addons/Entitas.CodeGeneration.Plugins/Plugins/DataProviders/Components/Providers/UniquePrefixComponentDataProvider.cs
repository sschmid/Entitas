using System;
using System.Linq;
using Entitas.CodeGenerator.Attributes;

namespace Entitas.CodeGenerator {

    public class UniquePrefixComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            data.SetUniqueComponentPrefix(getUniqueComponentPrefix(type));
        }

        string getUniqueComponentPrefix(Type type) {
            var attr = Attribute.GetCustomAttributes(type)
                                .OfType<CustomPrefixAttribute>()
                                .SingleOrDefault();

            return attr == null ? "is" : attr.prefix;
        }
    }

    public static class UniquePrefixComponentDataExtension {

        public const string COMPONENT_UNIQUE_PREFIX = "component_uniquePrefix";

        public static string GetUniqueComponentPrefix(this ComponentData data) {
            return (string)data[COMPONENT_UNIQUE_PREFIX];
        }

        public static void SetUniqueComponentPrefix(this ComponentData data, string prefix) {
            data[COMPONENT_UNIQUE_PREFIX] = prefix;
        }
    }
}
