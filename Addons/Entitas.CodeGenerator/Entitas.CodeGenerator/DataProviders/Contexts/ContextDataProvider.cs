using System.Linq;

namespace Entitas.CodeGenerator {

    public class ContextDataProvider : ICodeGeneratorDataProvider {

        public string name { get { return "Context"; } }
        public int priority { get { return 0; } }
        public bool isEnabledByDefault { get { return true; } }

        string[] _contextNames;

        public ContextDataProvider() : this(null) {
        }

        public ContextDataProvider(string[] contextNames) {
            _contextNames = contextNames;
        }

        public CodeGeneratorData[] GetData() {
            if(_contextNames == null) {
                var config = new CodeGeneratorConfig(EntitasPreferences.LoadConfig());
                _contextNames = config.contexts;
            }

            return _contextNames
                .Select(contextName => {
                    var data = new ContextData();
                    data.SetContextName(contextName);
                    return data;
                }).ToArray();
        }
    }

    public static class ContextDataExtension {

        public const string CONTEXT_NAME = "context_name";

        public static string GetContextName(this ContextData data) {
            return (string)data[CONTEXT_NAME];
        }

        public static void SetContextName(this ContextData data, string contextName) {
            data[CONTEXT_NAME] = contextName;
        }
    }
}
