using System.Linq;

namespace Entitas.CodeGenerator {

    public class ContextAttributesGenerator : IContextCodeGenerator {

        public CodeGenFile[] Generate(string[] contextNames) {
            return contextNames
                .Select(contextName => contextName.UppercaseFirst())
                .Select(contextName => new CodeGenFile(
                    contextName + "Attribute",
                    generateContextAttributes(contextName),
                    GetType().FullName
                )).ToArray();
        }

        static string generateContextAttributes(string contextName) {
            return string.Format(@"using Entitas.CodeGenerator;

public class {0}Attribute : ContextAttribute {{

    public {0}Attribute() : base(""{0}"") {{
    }}
}}

", contextName);
        }
    }
}
