using System;
using System.Linq;
using Entitas.CodeGenerator.Api;

namespace Entitas.CodeGenerator {

    public class ContextsComponentDataProvider : IComponentDataProvider {

        readonly string _defaultContextName;

        public ContextsComponentDataProvider(string defaultContextName) {
            _defaultContextName = defaultContextName;
        }

        public void Provide(Type type, ComponentData data) {
            var contextNames = GetContextNames(type);
            if(contextNames.Length == 0) {
                contextNames = new [] { _defaultContextName };
            }
            data.SetContextNames(contextNames);
        }

        public static string[] GetContextNames(Type type) {
            return Attribute
                .GetCustomAttributes(type)
                .OfType<ContextAttribute>()
                .Select(attr => attr.contextName)
                .ToArray();
        }
    }

    public static class ContextsComponentDataProviderExtension {

        public const string COMPONENT_CONTEXTS = "component_contexts";

        public static string[] GetContextNames(this ComponentData data) {
            return (string[])data[COMPONENT_CONTEXTS];
        }

        public static void SetContextNames(this ComponentData data, string[] contextNames) {
            data[COMPONENT_CONTEXTS] = contextNames;
        }
    }
}
