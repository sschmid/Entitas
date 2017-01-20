using System.Linq;

namespace Entitas.CodeGenerator {

    public static class ContextCodeGeneratorDataExtension {

        public static string GetContextName(this CodeGeneratorData data) {
            return (string)data[ContextDataProvider.CONTEXT_NAME];
        }
    }

    public class ContextDataProvider : ICodeGeneratorDataProvider {

        public const string CONTEXT_NAME = "context_name";

        readonly string[] _contextNames;

        public ContextDataProvider(string[] contextNames) {
            _contextNames = contextNames
                .Select(contextName => contextName.UppercaseFirst())
                .Distinct()
                .OrderBy(contextName => contextName)
                .ToArray();
        }

        public CodeGeneratorData[] GetData() {
            return _contextNames
                .Select(contextName => {
                    var data = new CodeGeneratorData(GetType().FullName);
                    data[CONTEXT_NAME] = contextName;
                    return data;
                }).ToArray();
        }
    }
}
