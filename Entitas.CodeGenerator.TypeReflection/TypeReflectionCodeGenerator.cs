using System.Reflection;

namespace Entitas.CodeGenerator.TypeReflection {
    public static class TypeReflectionCodeGenerator {
        public static void Generate(Assembly assembly, string directory, ICodeGenerator[] codeGenerators) {
            var provider = new TypeReflectionProvider(assembly.GetTypes());
            CodeGenerator.Generate(provider, directory, codeGenerators);
        }
    }
}

