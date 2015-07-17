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
            var assembly = Assembly.GetAssembly(typeof(ReadmeSnippets));
            var generatedFolder = getEntitasProjectDir() + "/Readme/Readme/Generated/";

            var codeGenerators = new ICodeGenerator[] {
                new ComponentExtensionsGenerator(),
                new IndicesLookupGenerator(),
                new PoolAttributeGenerator(),
                new PoolsGenerator(),
                new SystemExtensionsGenerator()
            };

            CodeGenerator.Generate(assembly.GetTypes(), new string[0], generatedFolder, codeGenerators);

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

