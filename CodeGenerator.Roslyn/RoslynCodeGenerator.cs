using CodeGenerator.Roslyn.PostProcessors;
using CodeGenerator.Roslyn.Providers;
using CSharpCodeGenerator;
using Entitas.CodeGeneration;

namespace CodeGenerator.Roslyn
{
    public static class RoslynCodeGenerator
    {
        public static CodeGenFile [] Generate (string projectPath, string [] poolNames, string [] blueprintNames, string directory, ICodeGenerator [] codeGenerators)
        {
            System.Console.WriteLine ("Generate...");
            // TODO create RoslynProvider
            var project = ProjectStructure.Load(projectPath);
            var provider = new RoslynComponentInfoProvider(project, poolNames, blueprintNames);
            IPostProcessor [] postProcessors = {
                new AddHeaderToFileProcessor(),
                new DeployToProjectProcessor(projectPath, directory), 
            };
            return Entitas.CodeGeneration.CodeGenerator.Generate (provider, codeGenerators, postProcessors);
        }
    }
}

