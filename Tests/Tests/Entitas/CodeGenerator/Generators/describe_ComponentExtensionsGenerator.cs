using System;
using Entitas.CodeGenerator;
using My.Namespace;
using NSpec;

class describe_ComponentExtensionsGenerator : nspec {

    const bool logResults = false;

    const string classSuffix = "GeneratedExtension";

    void generates(ComponentInfo componentInfo, string expectedFileContent) {
        expectedFileContent = expectedFileContent.ToUnixLineEndings();
        var files = new ComponentExtensionsGenerator().Generate(new[] { componentInfo });
        var expectedFilePath = componentInfo.fullTypeName + classSuffix;

        files.Length.should_be(1);
        var file = files[0];

        #pragma warning disable
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
        it["supports properties"] = () => generates(ComponentWithFieldsAndProperties.componentInfo, ComponentWithFieldsAndProperties.extensions);
        it["ignores [DontGenerate]"] = () => {
            var componentInfo = DontGenerateComponent.componentInfo;
            var files = new ComponentExtensionsGenerator().Generate(new[] { componentInfo });
            files.Length.should_be(0);
        };

        it["works with namespaces"] = () => generates(NamespaceComponent.componentInfo, NamespaceComponent.extensions);
        it["generates matchers for each pool"] = () => generates(CComponent.componentInfo, CComponent.extensions);
        it["generates custom prefix"] = () => generates(CustomPrefixComponent.componentInfo, CustomPrefixComponent.extensions);
        it["generates component with default pool"] = () => generates(DefaultPoolComponent.componentInfo, DefaultPoolComponent.extensions);
        it["generates component with default pool and others"] = () => generates(MultiplePoolAndDefaultPoolComponent.componentInfo, MultiplePoolAndDefaultPoolComponent.extensions);

        it["generates component for class"] = () => generates(SomeClass.componentInfo, SomeClass.extensions);
        it["generates component for struct"] = () => generates(SomeStruct.componentInfo, SomeStruct.extensions);
    }
}

