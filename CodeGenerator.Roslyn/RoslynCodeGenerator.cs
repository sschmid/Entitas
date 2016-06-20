using System.Reflection;

namespace Entitas.CodeGeneration
{
    public static class RoslynCodeGenerator
    {
        public static CodeGenFile [] Generate (Assembly assembly, string [] poolNames, string [] blueprintNames, string directory, ICodeGenerator [] codeGenerators)
        {
            System.Console.WriteLine ("Generate...");
            // TODO create RoslynProvider
            //var provider = new TypeReflectionProvider (assembly.GetTypes (), poolNames, blueprintNames);
            //IPostProcessor [] postProcessors = {
            //    new AddHeaderToFileProcessor(),
            //    new WriteToDirectoryProcessor(directory)
            //};
            //return CodeGenerator.Generate (provider, codeGenerators, postProcessors);
            return null;
        }
    }
}

