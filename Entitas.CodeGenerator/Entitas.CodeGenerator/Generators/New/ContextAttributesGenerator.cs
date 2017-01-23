using System.IO;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class ContextAttributesGenerator : ICodeGenerator {

        const string attributeTemplate =
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
                attributeTemplate.Replace("${Context}", contextName),
                GetType().FullName
            );
        }
    }
}
