using System.IO;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class EntityGenerator : ICodeGenerator {

        public bool IsEnabledByDefault { get { return true; } }

        const string ENTITY_TEMPLATE =
@"using Entitas;

public sealed partial class ${Context}Entity : Entity {
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
                ENTITY_TEMPLATE.Replace("${Context}", contextName),
                GetType().FullName
            );
        }
    }
}
