using System;
using System.IO;
using Entitas.CodeGenerator;
using My.Namespace;

namespace Tests {

    public static class GenerateTestComponents {

        public static void Generate() {
            var generatedFolder = getEntitasProjectDir() + "/Tests/Tests/Entitas/CodeGenerator/Fixtures/Generated/";

            var codeGenerators = new ICodeGenerator[] {
                new ComponentExtensionsGenerator(),
                new ComponentIndicesGenerator(),
                new ContextAttributesGenerator(),
                new ContextsGenerator(),
                new BlueprintsGenerator()
            };

            var types = new [] {
                typeof(AnimatingComponent),
                typeof(CComponent),
                typeof(ComponentWithFieldsAndProperties),
                typeof(CustomPrefixComponent),
                typeof(MovableComponent),
                typeof(NamespaceComponent),
                typeof(OtherContextComponent),
                typeof(PersonComponent),
                typeof(SomeClass),
                typeof(SomeClassHideInBlueprintInspector),
                typeof(SomeStruct),
                typeof(UserComponent)
            };

            var contexts = new [] {
                "ContextA",
                "ContextB",
                "ContextC",
                "OtherContext",
                "SomeContext",
                "SomeOtherContext"
            };

            var blueprints = new [] {
                "Gem",
                "Blocker"
            };

            var provider = new TypeReflectionProvider(types, contexts, blueprints);
            var files = CodeGenerator.Generate(provider, generatedFolder, codeGenerators);

            foreach(var file in files) {
                Console.WriteLine("Generated: " + file.fileName);
            }

            Console.WriteLine("Done. Press any key...");
            Console.Read();
        }

        static string getEntitasProjectDir() {
            var dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            const string projectName = "Tests";
            while(dirInfo.Name != projectName) {
                dirInfo = dirInfo.Parent;
            }

            return dirInfo.Parent.FullName;
        }
    }
}
