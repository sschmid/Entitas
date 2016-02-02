using System.Reflection;

namespace Entitas.CodeGenerator.TypeReflection {
    public static class TypeReflectionCodeGenerator {
        public static void Generate(Assembly assembly, string[] poolNames, string directory, ICodeGenerator[] codeGenerators) {
            var provider = new TypeReflectionProvider(assembly.GetTypes(), poolNames);
            CodeGenerator.Generate(provider, directory, codeGenerators);
        }
    }
}

