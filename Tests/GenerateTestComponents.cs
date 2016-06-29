using System;
using System.IO;
using System.Reflection;
using CodeGenerator.Reflection.Providers;
using Entitas.CodeGeneration;

class GenerateTestComponents {
    public static void Main(string[] args) {
        generate();
    }

    static void generate() {
        var assembly = Assembly.GetAssembly(typeof(SomeComponent));
        var generatedFolder = getEntitasProjectDir() + "/Tests/Tests/Entitas/CodeGenerator/Fixtures/Generated/";

        var codeGenerators = new ICodeGenerator[] {
            new ComponentExtensionsGenerator(),
            new ComponentIndicesGenerator(),
            new PoolAttributesGenerator(),
            new PoolsGenerator()
        };

        var provider = new TypeReflectionProvider(assembly.GetTypes(), new string[0], new string[0]);
        IPostProcessor[] postProcessors = {
            new AddHeaderToFileProcessor(),
            new WriteToDirectoryProcessor(generatedFolder)
        };

        var files = CodeGenerator.Generate(provider, codeGenerators, postProcessors);

        foreach (var file in files) {
            Console.WriteLine("Generated: " + file.fileName);
        }

        Console.WriteLine("Done. Press any key...");
        Console.Read();
    }

    static string getEntitasProjectDir() {
        var dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
        const string projectName = "Tests";
        while (dirInfo.Name != projectName) {
            dirInfo = dirInfo.Parent;
        }

        return dirInfo.Parent.FullName;
    }
}

