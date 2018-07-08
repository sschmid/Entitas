using System;
using System.Linq;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public class FlagPrefixComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            data.SetFlagPrefix(getFlagPrefix(type));
        }

        string getFlagPrefix(Type type) {
            var attr = Attribute.GetCustomAttributes(type)
                .OfType<FlagPrefixAttribute>()
                .SingleOrDefault();

            return attr == null ? "is" : attr.prefix;
        }
    }

    public static class FlagPrefixComponentDataExtension {

        public const string COMPONENT_FLAG_PREFIX = "Component.FlagPrefix";

        public static string GetFlagPrefix(this ComponentData data) {
            return (string)data[COMPONENT_FLAG_PREFIX];
        }

        public static void SetFlagPrefix(this ComponentData data, string prefix) {
            data[COMPONENT_FLAG_PREFIX] = prefix;
        }
    }
}
