using System.IO;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class ContextAttributeGenerator : ICodeGenerator {

        const string ATTRIBUTE_TEMPLATE =
@"using Entitas.CodeGenerator.Api;

public sealed class ${Context}Attribute : ContextAttribute {

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
