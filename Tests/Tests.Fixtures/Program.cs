using System.Reflection;
using Entitas.CodeGenerator;

namespace Tests.Fixtures {

    class MainClass {

        static readonly string[] contextNames = { "Test", "Test2" };
        const string path = "../../Generated";

        public static void Main(string[] args) {
            var types = Assembly
                .GetAssembly(typeof(MainClass))
                .GetTypes();

            var dataProviders = new ICodeGeneratorDataProvider[] {
                new ContextDataProvider(contextNames),
                new ComponentDataProvider(types)
            };

            var codeGenerators = new ICodeGenerator[] {
                new EntityGenerator(),
                new ContextGenerator(),
                new ContextAttributeGenerator(),
                new ComponentsLookupGenerator(),
                new ComponentEntityGenerator(),
                new ComponentContextGenerator(),
                new ComponentGenerator(),
                new MatcherGenerator()
            };

            var postProcessors = new ICodeGenFilePostProcessor [] {
                new AddFileHeaderPostProcessor(),
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
    }
}
