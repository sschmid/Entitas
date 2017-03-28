using System;
using System.Linq;
using Entitas.CodeGenerator.Attributes;

namespace Entitas.CodeGenerator {

    public class ContextsComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            var contextNames = GetContextNames(type);
            data.SetContextNames(contextNames);
        }

        public static string[] GetContextNames(Type type) {
            var contextNames = Attribute
                .GetCustomAttributes(type)
                .OfType<ContextAttribute>()
                .Select(attr => attr.contextName)
                .ToArray();

            if(contextNames.Length == 0) {
                var config = new CodeGeneratorConfig(EntitasPreferences.LoadConfig(EntitasPreferences.GetConfigPath()));
                contextNames = new [] { config.contexts[0] };
            }

            return contextNames;
        }
    }

    public static class ContextsComponentDataExtension {

        public const string COMPONENT_CONTEXTS = "component_contexts";

        public static string[] GetContextNames(this ComponentData data) {
            return (string[])data[COMPONENT_CONTEXTS];
        }

        public static void SetContextNames(this ComponentData data, string[] contextNames) {
            data[COMPONENT_CONTEXTS] = contextNames;
        }
    }
}
