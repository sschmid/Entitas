using System;
using System.IO;
using System.Linq;
using Entitas.CodeGenerator;
using My.Namespace;
using NSpec;

class describe_ComponentExtensionsGenerator : nspec {

    const bool logResults = false;

    const string classSuffix = "GeneratedExtension";

    static void generates<T>(string expectedFileName = null) {
        var entitasDir = new DirectoryInfo(Directory.GetCurrentDirectory());
        var fixturesDir = entitasDir + "/Tests/Tests/Entitas/CodeGenerator/Fixtures/Generated/";
        expectedFileName = (expectedFileName ?? typeof(T).FullName) + "GeneratedExtension";
        var path = fixturesDir + expectedFileName + ".cs";

        var expectedFileContent = File.ReadAllText(path);
        var info = TypeReflectionProvider.GetComponentInfos(typeof(T)).Single();

        var codeGenFiles = new ComponentExtensionsGenerator().Generate(new[] { info });
        codeGenFiles.Length.should_be(1);
        var codeGenFile = codeGenFiles.Single();

        #pragma warning disable
        if (logResults) {
            Console.WriteLine("should:\n" + expectedFileContent);
            Console.WriteLine("was:\n" + codeGenFile.fileContent);
        }

        codeGenFile.fileName.should_be(expectedFileName);
        codeGenFile.fileContent.should_be(expectedFileContent);
    }

    void when_generating() {
        it["component without fields"] = () => generates<MovableComponent>();
        it["component with fields"] = () => generates<PersonComponent>();
        it["single component without fields"] = () => generates<AnimatingComponent>();
        it["single component with fields"] = () => generates<UserComponent>();
        it["component for custom pool"] = () => generates<OtherPoolComponent>();
        it["supports properties"] = () => generates<ComponentWithFieldsAndProperties>();
        it["ignores [DontGenerate]"] = () => {
            var info = TypeReflectionProvider.GetComponentInfos(typeof(DontGenerateComponent))[0];
            var files = new ComponentExtensionsGenerator().Generate(new[] { info });
            files.Length.should_be(0);
        };

        it["works with namespaces"] = () => generates<NamespaceComponent>();
        it["generates matchers for each pool"] = () => generates<CComponent>();
        it["generates custom prefix"] = () => generates<CustomPrefixComponent>();
        it["generates component with default pool"] = () => generates<DefaultPoolComponent>();
        it["generates component with default pool and others"] = () => generates<MultiplePoolAndDefaultPoolComponent>();

        it["generates component for class"] = () => generates<SomeClass>("SomeClassComponent");
        it["generates component for struct"] = () => generates<SomeStruct>("SomeStructComponent");
    }
}

