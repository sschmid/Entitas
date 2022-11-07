using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.CodeGeneration.Plugins
{
    public class EntityGenerator : ICodeGenerator
    {
        public string Name => "Entity";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string TEMPLATE =
            @"public sealed partial class ${EntityType} : Entitas.Entity {
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
                contextName.AddEntitySuffix() + ".cs",
                TEMPLATE.Replace(contextName),
                GetType().FullName
            );
        }
    }
}
