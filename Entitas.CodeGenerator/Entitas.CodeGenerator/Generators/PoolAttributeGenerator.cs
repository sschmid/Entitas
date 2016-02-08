using System.Linq;

namespace Entitas.CodeGenerator {
    public class PoolAttributeGenerator : IPoolCodeGenerator {

        public CodeGenFile[] Generate(string[] poolNames) {
            return poolNames.Select(poolName => new CodeGenFile {
                fileName = poolName + "Attribute",
                fileContent = generatePoolAttributes(poolName).ToUnixLineEndings()
            }).ToArray();
        }

        static string generatePoolAttributes(string poolName) {
            var fileContent = new CSharpFileBuilder();
            var classDesc = fileContent.NoNamespace().AddClass(poolName + "Attribute").AddModifier(AccessModifiers.Public);
            classDesc.SetBaseClass("Entitas.CodeGenerator.PoolAttribute");
            classDesc.AddConstructor().AddModifier(AccessModifiers.Public).CallBase("\"" + poolName + "\"");

            return fileContent.ToString();
        }
    }
}

