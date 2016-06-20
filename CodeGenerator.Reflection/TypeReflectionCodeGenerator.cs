using System.Reflection;

namespace Entitas.CodeGeneration {
    public static class TypeReflectionCodeGenerator {
        public static CodeGenFile[] Generate(Assembly assembly, string[] poolNames, string[] blueprintNames, string directory, ICodeGenerator[] codeGenerators) {
            System.Console.WriteLine("Generate...");
            var provider = new TypeReflectionProvider(assembly.GetTypes(), poolNames, blueprintNames);
            IPostProcessor[] postProcessors = {
                new AddHeaderToFileProcessor(),
                new WriteToDirectoryProcessor(directory)
            };
            return CodeGenerator.Generate(provider, codeGenerators, postProcessors);
        }
    }
}

