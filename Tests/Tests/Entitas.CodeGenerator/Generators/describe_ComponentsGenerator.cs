using System;
using Entitas.CodeGenerator;
using My.Namespace;
using NSpec;

class describe_ComponentsGenerator : nspec {

    const bool logResults = false;

    const string classSuffix = "GeneratedExtension";

    void generates(ComponentInfo componentInfo, string expectedFileContent) {
        expectedFileContent = expectedFileContent.ToUnixLineEndings();
        var files = new ComponentsGenerator().Generate(new[] { componentInfo });
        var expectedFilePath = componentInfo.fullTypeName + classSuffix;

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
        it["component without fields"] = () => generates(MovableComponent.componentInfo, MovableComponent.extensions);
        it["component with fields"] = () => generates(PersonComponent.componentInfo, PersonComponent.extensions);
        it["single component without fields"] = () => generates(AnimatingComponent.componentInfo, AnimatingComponent.extensions);
        it["single component with fields"] = () => generates(UserComponent.componentInfo, UserComponent.extensions);
        it["component for custom pool"] = () => generates(OtherPoolComponent.componentInfo, OtherPoolComponent.extensions);
        it["ignores [DontGenerate]"] = () => {
            var componentInfo = DontGenerateComponent.componentInfo;
            var files = new ComponentsGenerator().Generate(new[] { componentInfo });
            files.Length.should_be(0);
        };

        it["works with namespaces"] = () => generates(NamespaceComponent.componentInfo, NamespaceComponent.extensions);
        it["generates matchers for each pool"] = () => generates(CComponent.componentInfo, CComponent.extensions);
        it["generates custom prefix"] = () => generates(CustomPrefixComponent.componentInfo, CustomPrefixComponent.extensions);
    }
}

