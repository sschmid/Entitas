using System.Reflection;

namespace Entitas.CodeGenerator.TypeReflection {
    public static class TypeReflectionCodeGenerator {
        public static CodeGenFile[] Generate(Assembly assembly, string[] poolNames, string directory, ICodeGenerator[] codeGenerators) {
            var provider = new TypeReflectionProvider(assembly.GetTypes(), poolNames);
            return CodeGenerator.Generate(provider, directory, codeGenerators);
        }
    }
}

