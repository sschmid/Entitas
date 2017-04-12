using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.CodeGeneration.Attributes;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ContextsComponentDataProvider : IComponentDataProvider, IConfigurable {

        public Dictionary<string, string> defaultProperties { get { return _config.defaultProperties; } }

        readonly ContextNamesConfig _config = new ContextNamesConfig();

        public void Configure(Properties properties) {
            _config.Configure(properties);
        }

        public void Provide(Type type, ComponentData data) {
            var contextNames = GetContextNamesOrDefault(type);
            data.SetContextNames(contextNames);
        }

        public static string[] GetContextNames(Type type) {
            return Attribute
                .GetCustomAttributes(type)
                .OfType<ContextAttribute>()
                .Select(attr => attr.contextName)
                .ToArray();
        }

        public static string[] GetContextNamesOrDefault(Type type) {
            var contextNames = GetContextNames(type);
            if(contextNames.Length == 0) {
                var config = new ContextNamesConfig();
                config.Configure(Preferences.LoadProperties());
                contextNames = new [] { config.contextNames[0] };
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
