using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ContextDataProvider : ICodeGeneratorDataProvider, IConfigurable {

        public string name { get { return "Context"; } }
        public int priority { get { return 0; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _contextNamesConfig.defaultProperties; } }

        readonly ContextNamesConfig _contextNamesConfig = new ContextNamesConfig();

        public void Configure(Preferences preferences) {
            _contextNamesConfig.Configure(preferences);
        }

        public CodeGeneratorData[] GetData() {
            return _contextNamesConfig.contexts
                .Select(context => {
                    var data = new ContextData();
                    data.SetContextName(extractContextName(context));
                    data.SetKits(extractKits(context));
                    return data;
                }).ToArray();
        }

        static string extractContextName(string context) {
            return context
                .Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[0]
                .Trim();
        }

        static string[] extractKits(string context) {
            var contextSplit = context
                .Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            return contextSplit.Length > 1
                ? contextSplit[1]
                    .Split(new[] { "+" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(kit => kit.Trim())
                    .ToArray()
                : new string[0];
        }
    }

    public static class ContextDataExtension {

        public const string CONTEXT_NAME = "context_name";
        public const string CONTEXT_KITS = "context_kits";

        public static string GetContextName(this ContextData data) {
            return (string)data[CONTEXT_NAME];
        }

        public static void SetContextName(this ContextData data, string contextName) {
            data[CONTEXT_NAME] = contextName;
        }

        public static string[] GetKits(this ContextData data) {
            return (string[])data[CONTEXT_KITS];
        }

        public static void SetKits(this ContextData data, string[] kits) {
            data[CONTEXT_KITS] = kits;
        }
    }
}
