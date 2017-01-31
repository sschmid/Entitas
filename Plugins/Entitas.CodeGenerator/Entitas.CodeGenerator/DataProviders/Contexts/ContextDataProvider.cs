using System.Linq;

namespace Entitas.CodeGenerator {

    public class ContextDataProvider : ICodeGeneratorDataProvider {

        readonly string[] _contextNames;

        public ContextDataProvider() : this(new CodeGeneratorConfig(EntitasPreferences.LoadConfig()).contexts) {
        }

        public ContextDataProvider(string[] contextNames) {
            _contextNames = contextNames;
        }

        public CodeGeneratorData[] GetData() {
            return _contextNames
                .Select(contextName => {
                    var data = new ContextData();
                    data.SetContextName(contextName);
                    return data;
                }).ToArray();
        }
    }

    public static class ContextCodeGeneratorDataExtension {

        public const string CONTEXT_NAME = "context_name";

        public static string GetContextName(this ContextData data) {
            return (string)data[CONTEXT_NAME];
        }

        public static void SetContextName(this ContextData data, string contextName) {
            data[CONTEXT_NAME] = contextName;
        }
    }
}
