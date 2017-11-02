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
            return _contextNamesConfig.contextsWithKits
                .Select(kv => {
                    var data = new ContextData();
                    data.SetContextName(kv.Key);
                    data.SetKits(kv.Value);
                    return data;
                }).ToArray();
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
