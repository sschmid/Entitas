using System;
using System.IO;
using System.Reflection;
using Entitas.CodeGenerator;

class GenerateTestComponents {
    public static void Main(string[] args) {
        generate();
    }

    static void generate() {
        var assembly = Assembly.GetAssembly(typeof(TestExecuteSystem));
        var generatedFolder = getEntitasProjectDir() + "/Tests/Tests/Entitas.CodeGenerator/Fixtures/Generated/";

        var codeGenerators = new ICodeGenerator[] {
            new ComponentsGenerator(),
            new ComponentIndicesGenerator(),
            new PoolAttributeGenerator(),
            new PoolsGenerator()
        };

        CodeGenerator.Generate(assembly.GetTypes(), new string[0], generatedFolder, codeGenerators);

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

