using System.IO;
using System.Reflection;
using Entitas.CodeGenerator;

namespace Readme {

    class MainClass {

        public static void Main(string[] args) {
            generate();
        }

        static void generate() {
            var path = getEntitasProjectDir() + "/Readme/Readme/Generated/";

            var types = Assembly
                .GetAssembly(typeof(MainClass))
                .GetTypes();

            var contextNames = new [] { "Game", "GameState", "Input" };

            var dataProviders = new ICodeGeneratorDataProvider[] {
                new ContextDataProvider(contextNames),
                new ComponentDataProvider(types)
            };

            var codeGenerators = new ICodeGenerator[] {
                new ComponentContextGenerator(),
                new ComponentEntityGenerator(),
                new ComponentGenerator(),
                new ComponentsLookupGenerator(),
                new ContextAttributeGenerator(),
                new ContextGenerator(),
                new ContextsGenerator(),
                new EntityGenerator(),
                new MatcherGenerator()
            };

            var postProcessors = new ICodeGenFilePostProcessor [] {
                new MergeFilesPostProcessor(),
                new NewLinePostProcessor(),
                new WriteToDiskPostProcessor(path),
            };

            var codeGenerator = new CodeGenerator(dataProviders, codeGenerators, postProcessors);
            var files = codeGenerator.Generate();

            foreach(var file in files) {
                System.Console.WriteLine("file.fileName: " + file.fileName);
            }

            System.Console.WriteLine("Done. " + files.Length + " files generated.");
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
