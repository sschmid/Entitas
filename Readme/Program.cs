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
            var generatedFolder = getEntitasProjectDir() + "/Readme/Components/Generated/";

            var componentCodeGenerators = new IComponentCodeGenerator[] {
                new IndicesLookupGenerator(),
                new ComponentExtensionsGenerator()
            };

            var systemCodeGenerators = new ISystemCodeGenerator[] {
                new SystemExtensionsGenerator()
            };

            var poolCodeGenerators = new IPoolCodeGenerator[] {
                new PoolAttributeGenerator()
            };

            CodeGenerator.Generate(assembly.GetTypes(), new string[0], generatedFolder,
                componentCodeGenerators, systemCodeGenerators, poolCodeGenerators);

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

