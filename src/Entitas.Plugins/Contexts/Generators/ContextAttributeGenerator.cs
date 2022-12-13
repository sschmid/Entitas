using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.Plugins
{
    public class ContextAttributeGenerator : ICodeGenerator
    {
        public string Name => "Context (Attribute)";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string Template =
            @"public sealed class ${Context.Name}Attribute : Entitas.Plugins.Attributes.ContextAttribute
{
    public ${Context.Name}Attribute() : base(""${Context.Name}"") { }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ContextData>()
            .Select(d => Generate(d))
            .ToArray();

        CodeGenFile Generate(ContextData data) => new CodeGenFile(
            Path.Combine(data.Name, $"{data.Name}Attribute.cs"),
            data.ReplacePlaceholders(Template),
            GetType().FullName
        );
    }
}
