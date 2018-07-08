using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Serialization;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public class ContextsComponentDataProvider : IComponentDataProvider, IConfigurable {

        public Dictionary<string, string> defaultProperties { get { return _contextNamesConfig.defaultProperties; } }

        readonly ContextNamesConfig _contextNamesConfig = new ContextNamesConfig();

        public void Configure(Preferences preferences) {
            _contextNamesConfig.Configure(preferences);
        }

        public void Provide(Type type, ComponentData data) {
            var contextNames = GetContextNamesOrDefault(type);
            data.SetContextNames(contextNames);
        }

        public string[] GetContextNames(Type type) {
            return Attribute
                .GetCustomAttributes(type)
                .OfType<ContextAttribute>()
                .Select(attr => attr.contextName)
                .ToArray();
        }

        public string[] GetContextNamesOrDefault(Type type) {
            var contextNames = GetContextNames(type);
            if (contextNames.Length == 0) {
                contextNames = new[] { _contextNamesConfig.contextNames[0] };
            }

            return contextNames;
        }
    }

    public static class ContextsComponentDataExtension {

        public const string COMPONENT_CONTEXTS = "Component.ContextNames";

        public static string[] GetContextNames(this ComponentData data) {
            return (string[])data[COMPONENT_CONTEXTS];
        }

        public static void SetContextNames(this ComponentData data, string[] contextNames) {
            data[COMPONENT_CONTEXTS] = contextNames;
        }
    }
}
