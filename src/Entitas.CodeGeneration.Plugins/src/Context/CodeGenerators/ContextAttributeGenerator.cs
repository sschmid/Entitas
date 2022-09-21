using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.CodeGeneration.Plugins
{
    public class ContextAttributeGenerator : ICodeGenerator
    {
        public string Name => "Context (Attribute)";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string TEMPLATE =
            @"public sealed class ${ContextName}Attribute : Entitas.CodeGeneration.Attributes.ContextAttribute {

    public ${ContextName}Attribute() : base(""${ContextName}"") {
    }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ContextData>()
            .Select(generate)
            .ToArray();

        CodeGenFile generate(ContextData data)
        {
            var contextName = data.GetContextName();
            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar +
                contextName + "Attribute.cs",
                TEMPLATE.Replace(contextName),
                GetType().FullName
            );
        }
    }
}
