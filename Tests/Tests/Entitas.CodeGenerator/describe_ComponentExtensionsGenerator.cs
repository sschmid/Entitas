using NSpec;
using Entitas.CodeGenerator;
using System;
using My.Namespace;

class describe_ComponentExtensionsGenerator : nspec {

    bool logResults = false;

    const string classSuffix = "GeneratedExtension";

    void generates(Type type, string expectedFileContent) {
        expectedFileContent = expectedFileContent.ToUnixLineEndings();
        var files = new ComponentExtensionsGenerator().Generate(new[] { type });
        var expectedFilePath = type + classSuffix;

        files.Length.should_be(1);
        var file = files[0];

        if (logResults) {
            Console.WriteLine("should:\n" + expectedFileContent);
            Console.WriteLine("was:\n" + file.fileContent);
        }

        file.fileName.should_be(expectedFilePath);
        file.fileContent.should_be(expectedFileContent);
    }

    void when_generating() {
        it["component without fields"] = () => generates(typeof(MovableComponent), MovableComponent.extensions);
        it["component with fields"] = () => generates(typeof(PersonComponent), PersonComponent.extensions);
        it["single singleton component"] = () => generates(typeof(AnimatingComponent), AnimatingComponent.extensions);
        it["single component with fields"] = () => generates(typeof(UserComponent), UserComponent.extensions);
        it["component for custom pool"] = () => generates(typeof(OtherPoolComponent), OtherPoolComponent.extensions);
        it["ignores [DontGenerate]"] = () => {
            var type = typeof(DontGenerateComponent);
            var files = new ComponentExtensionsGenerator().Generate(new[] { type });
            files.Length.should_be(0);
        };

        it["works with namespaces"] = () => generates(typeof(NamespaceComponent), NamespaceComponent.extensions);
        it["generates matchers for each pool"] = () => generates(typeof(CComponent), CComponent.extensions);
        it["generates custom prefix"] = () => generates(typeof(CustomPrefixComponent), CustomPrefixComponent.extensions);
    }
}

