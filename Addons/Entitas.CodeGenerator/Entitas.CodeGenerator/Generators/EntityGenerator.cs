using System.IO;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class EntityGenerator : ICodeGenerator {

        public string name { get { return "Entity"; } }
        public bool isEnabledByDefault { get { return true; } }

        const string ENTITY_TEMPLATE =
@"public sealed partial class ${ContextName}Entity : Entitas.Entity {
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ContextData>()
                .Select(d => generateEntityClass(d))
                .ToArray();
        }

        CodeGenFile generateEntityClass(ContextData data) {
            var contextName = data.GetContextName();
            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + contextName + "Entity.cs",
                ENTITY_TEMPLATE.Replace("${ContextName}", contextName),
                GetType().FullName
            );
        }
    }
}
