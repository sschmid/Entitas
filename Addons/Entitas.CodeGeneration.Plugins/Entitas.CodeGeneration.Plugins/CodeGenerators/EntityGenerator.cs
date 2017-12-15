using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins {

    public class EntityGenerator : ICodeGenerator {

        public string name { get { return "Entity"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        const string ENTITY_TEMPLATE =
@"public sealed partial class ${ContextName}Entity : Entitas.Entity {
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ContextData>()
                .Select(generateEntityClass)
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
