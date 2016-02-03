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
            var generatedFolder = getEntitasProjectDir() + "/Readme/Readme/Generated/";

            var codeGenerators = new ICodeGenerator[] {
                new ComponentsGenerator(),
                new ComponentIndicesGenerator(),
                new PoolAttributeGenerator(),
                new PoolsGenerator()
            };

            var assembly = Assembly.GetAssembly(typeof(ReadmeSnippets));
            var provider = new TypeReflectionProvider(assembly.GetTypes(), new string[0]);
            CodeGenerator.Generate(provider, generatedFolder, codeGenerators);

            Console.WriteLine("Done. Press any key...");
            Console.Read();
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

