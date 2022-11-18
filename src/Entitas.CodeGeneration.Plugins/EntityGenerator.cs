using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.Plugins
{
    public class EntityGenerator : ICodeGenerator
    {
        public string Name => "Entity";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string Template =
            @"public sealed partial class ${EntityType} : Entitas.Entity {
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
                Path.Combine(context, $"{context.AddEntitySuffix()}.cs"),
                Template.Replace(context),
                GetType().FullName
            );
        }
    }
}
