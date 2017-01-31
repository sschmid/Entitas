using System;
using System.IO;
using System.Reflection;
using Entitas.CodeGenerator;

namespace Readme {
    class MainClass {
        public static void Main(string[] args) {
            generate();
        }

        static void generate() {
            //var generatedFolder = getEntitasProjectDir() + "/Readme/Readme/Generated/";

            //var codeGenerators = new ICodeGenerator[] {
            //    new ComponentExtensionsGenerator(),
            //    new ComponentIndicesGenerator(),
            //    new ContextAttributesGenerator(),
            //    new ContextsGenerator()
            //};

            //var assembly = Assembly.GetAssembly(typeof(ReadmeSnippets));

            //var provider = new TypeReflectionProvider(assembly.GetTypes(), new string[0], new string[0]);
            //var files = CodeGenerator.Generate(provider, generatedFolder, codeGenerators);

            //foreach(var file in files) {
            //    Console.WriteLine("Generated: " + file.fileName);
            //}

            //Console.WriteLine("Done. Press any key...");
            //Console.Read();
        }

        static string getEntitasProjectDir() {
            var dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            const string projectName = "Readme";
            while (dirInfo.Name != projectName) {
                dirInfo = dirInfo.Parent;
            }

            return dirInfo.Parent.FullName;
        }
    }
}
