using System.IO;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class ContextAttributeGenerator : ICodeGenerator {

        public string name { get { return "Context Attributes"; } }
        public bool isEnabledByDefault { get { return true; } }

        const string ATTRIBUTE_TEMPLATE =
@"public sealed class ${Context}Attribute : Entitas.CodeGenerator.Api.ContextAttribute {

    public ${Context}Attribute() : base(""${Context}"") {
    }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ContextData>()
                .Select(d => generateAttributeClass(d))
                .ToArray();
        }

        CodeGenFile generateAttributeClass(ContextData data) {
            var contextName = data.GetContextName();
            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + contextName + "Attribute.cs",
                ATTRIBUTE_TEMPLATE.Replace("${Context}", contextName),
                GetType().FullName
            );
        }
    }
}
