using System.Reflection;
using CodeGenerator.Reflection.Providers;
using Entitas.CodeGeneration;

namespace CodeGenerator.Reflection {
    public static class TypeReflectionCodeGenerator {
        public static CodeGenFile[] Generate(Assembly assembly, string[] poolNames, string[] blueprintNames, string directory, ICodeGenerator[] codeGenerators) {
            System.Console.WriteLine("Generate...");
            var provider = new TypeReflectionProvider(assembly.GetTypes(), poolNames, blueprintNames);
            IPostProcessor[] postProcessors = {
                new AddHeaderToFileProcessor(),
                new WriteToDirectoryProcessor(directory)
            };
            return Entitas.CodeGeneration.CodeGenerator.Generate(provider, codeGenerators, postProcessors);
        }
    }
}

