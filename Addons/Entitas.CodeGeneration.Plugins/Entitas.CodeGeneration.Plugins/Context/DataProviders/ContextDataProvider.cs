using System.Collections.Generic;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;

namespace Entitas.CodeGeneration.Plugins {

    public class ContextDataProvider : IDataProvider, IConfigurable {

        public string Name { get { return "Context"; } }
        public int Order { get { return 0; } }
        public bool RunInDryMode { get { return true; } }

        public Dictionary<string, string> DefaultProperties { get { return _contextNamesConfig.DefaultProperties; } }

        readonly ContextNamesConfig _contextNamesConfig = new ContextNamesConfig();

        public void Configure(Preferences preferences) {
            _contextNamesConfig.Configure(preferences);
        }

        public CodeGeneratorData[] GetData() {
            return _contextNamesConfig.contextNames
                .Select(contextName => {
                    var data = new ContextData();
                    data.SetContextName(contextName);
                    return data;
                }).ToArray();
        }
    }

    public static class ContextDataExtension {

        public const string CONTEXT_NAME = "Context.Name";

        public static string GetContextName(this ContextData data) {
            return (string)data[CONTEXT_NAME];
        }

        public static void SetContextName(this ContextData data, string contextName) {
            data[CONTEXT_NAME] = contextName;
        }
    }
}
