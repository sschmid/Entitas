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
        var projectRoot = TestExtensions.GetProjectRoot();
        var fixturesDir = projectRoot + "/Tests/Tests/Entitas/CodeGenerator/Fixtures/Generated/";
        expectedFileName = (expectedFileName ?? typeof(T).FullName) + "GeneratedExtension";
        var path = fixturesDir + expectedFileName + ".cs";

        var expectedFileContent = File.ReadAllText(path);
        var info = TypeReflectionProvider.GetComponentInfos(typeof(T)).Single();

        var codeGenFiles = new ComponentExtensionsGenerator().Generate(new [] { info });
        codeGenFiles.Length.should_be(1);
        var codeGenFile = codeGenFiles.Single();

        #pragma warning disable
        if(logResults) {
            Console.WriteLine("should:\n" + expectedFileContent);
            Console.WriteLine("was:\n" + codeGenFile.fileContent);
        }

        codeGenFile.fileName.should_be(expectedFileName);
        var header = string.Format(CodeGenerator.AUTO_GENERATED_HEADER_FORMAT, typeof(ComponentExtensionsGenerator));
        (header + codeGenFile.fileContent).should_be(expectedFileContent);
    }

    void when_generating() {
        it["component without fields"] = () => generates<MovableComponent>();
        it["component with fields"] = () => generates<PersonComponent>();
        it["single component without fields"] = () => generates<AnimatingComponent>();
        it["single component with fields"] = () => generates<UserComponent>();
        it["component for custom context"] = () => generates<OtherContextComponent>();
        it["supports properties"] = () => generates<ComponentWithFieldsAndProperties>();
        it["ignores [DontGenerate]"] = () => {
            var info = TypeReflectionProvider.GetComponentInfos(typeof(DontGenerateComponent))[0];
            var files = new ComponentExtensionsGenerator().Generate(new [] { info });
            files.Length.should_be(0);
        };

        it["works with namespaces"] = () => generates<NamespaceComponent>();
        it["generates matchers for each context"] = () => generates<CComponent>();
        it["generates custom prefix"] = () => generates<CustomPrefixComponent>();
        it["generates component with default context"] = () => generates<DefaultContextComponent>();
        it["generates component with default context and others"] = () => generates<MultipleContextAndDefaultContextComponent>();

        it["generates component for class"] = () => generates<SomeClass>("SomeClassComponent");
        it["generates component for struct"] = () => generates<SomeStruct>("SomeStructComponent");

        it["generates component for class with HideInBlueprintInspectorAttribute"] = () =>
            generates<SomeClassHideInBlueprintInspector>("SomeClassHideInBlueprintInspectorComponent");
    }
}
