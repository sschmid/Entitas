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

        const string Template =
            @"public sealed class ${Context}Attribute : Entitas.CodeGeneration.Attributes.ContextAttribute {

    public ${Context}Attribute() : base(""${Context}"") {
    }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ContextData>()
            .Select(d => Generate(d))
            .ToArray();

        CodeGenFile Generate(ContextData data)
        {
            var context = data.Name;
            return new CodeGenFile(
                Path.Combine(context, $"{context}Attribute.cs"),
                Template.Replace(context),
                GetType().FullName
            );
        }
    }
}
