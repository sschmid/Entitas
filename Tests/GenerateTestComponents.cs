using System;
using System.IO;
using System.Reflection;
using Entitas.CodeGenerator;
using My.Namespace;

namespace Tests {

    public static class GenerateTestComponents {

        public static void Generate() {
            var generatedFolder = getEntitasProjectDir() + "/Tests/Tests/Entitas/CodeGenerator/Fixtures/Generated/";

            var codeGenerators = new ICodeGenerator[] {
                new ComponentExtensionsGenerator(),
                new ComponentIndicesGenerator(),
                new PoolAttributesGenerator(),
                new PoolsGenerator(),
                new BlueprintsGenerator()
            };

            var types = new [] {
                typeof(AnimatingComponent),
                typeof(CComponent),
                typeof(ComponentWithFieldsAndProperties),
                typeof(CustomPrefixComponent),
                typeof(DefaultPoolComponent),
                typeof(MovableComponent),
                typeof(MultiplePoolAndDefaultPoolComponent),
                typeof(NamespaceComponent),
                typeof(OtherPoolComponent),
                typeof(PersonComponent),
                typeof(SomeClass),
                typeof(SomeClassHideInBlueprintInspector),
                typeof(SomeStruct),
                typeof(UserComponent)
            };

            var pools = new [] {
                "PoolA",
                "PoolB",
                "PoolC",
                "OtherPool",
                "SomePool",
                "SomeOtherPool"
            };

            var blueprints = new [] {
                "Gem",
                "Blocker"
            };

            var provider = new TypeReflectionProvider(types, pools, blueprints);
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
