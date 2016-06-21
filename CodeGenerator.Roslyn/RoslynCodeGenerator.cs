using System.Reflection;
using CSharpCodeGenerator;
using Entitas.CodeGenerator;

namespace Entitas.CodeGeneration
{
    public static class RoslynCodeGenerator
    {
        public static CodeGenFile [] Generate (ProjectStructure project, string [] poolNames, string [] blueprintNames, string directory, ICodeGenerator [] codeGenerators)
        {
            System.Console.WriteLine ("Generate...");
            // TODO create RoslynProvider
            var provider = new RoslynComponentInfoProvider(project, poolNames, blueprintNames);
            IPostProcessor [] postProcessors = {
                new AddHeaderToFileProcessor(),
                new WriteToDirectoryProcessor(directory)
            };
            return CodeGenerator.Generate (provider, codeGenerators, postProcessors);
        }
    }
}

