using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins {

    public class EntityGenerator : ICodeGenerator {

        public string name { get { return "Entity"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        const string TEMPLATE = @"public sealed class ${EntityType} : Entitas.Entity
{
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ContextData>()
                .Select(generate)
                .ToArray();
        }

        CodeGenFile generate(ContextData data) {
            var contextName = data.GetContextName();
            return new CodeGenFile(
                "Entity".ToFileName(contextName),
                TEMPLATE.Replace(contextName),
                GetType().FullName
            );
        }
    }
}
