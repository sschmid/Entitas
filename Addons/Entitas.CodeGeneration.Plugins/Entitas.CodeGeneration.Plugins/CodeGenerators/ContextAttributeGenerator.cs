using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins {

    public class ContextAttributeGenerator : ICodeGenerator {

        public string name { get { return "Context Attribute"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        const string ATTRIBUTE_TEMPLATE =
@"public sealed class ${ContextName}Attribute : Entitas.CodeGeneration.Attributes.ContextAttribute {

    public ${ContextName}Attribute() : base(""${ContextName}"") {
    }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ContextData>()
                .Select(generateAttributeClass)
                .ToArray();
        }

        CodeGenFile generateAttributeClass(ContextData data) {
            var contextName = data.GetContextName();
            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + contextName + "Attribute.cs",
                ATTRIBUTE_TEMPLATE.Replace("${ContextName}", contextName),
                GetType().FullName
            );
        }
    }
}
