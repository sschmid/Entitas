using System;
using System.Linq;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public class UniquePrefixComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            data.SetUniquePrefix(getUniquePrefix(type));
        }

        string getUniquePrefix(Type type) {
            var attr = Attribute.GetCustomAttributes(type)
                .OfType<UniquePrefixAttribute>()
                .SingleOrDefault();

            return attr == null ? "is" : attr.prefix;
        }
    }

    public static class UniquePrefixComponentDataExtension {

        public const string COMPONENT_UNIQUE_PREFIX = "Component.UniquePrefix";

        public static string GetUniquePrefix(this ComponentData data) {
            return (string)data[COMPONENT_UNIQUE_PREFIX];
        }

        public static void SetUniquePrefix(this ComponentData data, string prefix) {
            data[COMPONENT_UNIQUE_PREFIX] = prefix;
        }
    }
}
