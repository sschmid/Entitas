using System;
using Entitas.CodeGenerator;
using My.Namespace;
using NSpec;

class describe_ComponentExtensionsGenerator : nspec {

    const bool logResults = false;

    const string classSuffix = "GeneratedExtension";

    void generates<T>(string expectedFileContent) {
        expectedFileContent = expectedFileContent.ToUnixLineEndings();

        var info = TypeReflectionProvider.GetComponentInfos(typeof(T))[0];
        var files = new ComponentExtensionsGenerator().Generate(new[] { info });
        var expectedFilePath = info.fullTypeName + classSuffix;

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
        it["component without fields"] = () => generates<MovableComponent>(MovableComponent.extensions);
        it["component with fields"] = () => generates<PersonComponent>(PersonComponent.extensions);
        it["single component without fields"] = () => generates<AnimatingComponent>(AnimatingComponent.extensions);
        it["single component with fields"] = () => generates<UserComponent>(UserComponent.extensions);
        it["component for custom pool"] = () => generates<OtherPoolComponent>(OtherPoolComponent.extensions);
        it["supports properties"] = () => generates<ComponentWithFieldsAndProperties>(ComponentWithFieldsAndProperties.extensions);
        it["ignores [DontGenerate]"] = () => {
            var info = TypeReflectionProvider.GetComponentInfos(typeof(DontGenerateComponent))[0];
            var files = new ComponentExtensionsGenerator().Generate(new[] { info });
            files.Length.should_be(0);
        };

        it["works with namespaces"] = () => generates<NamespaceComponent>(NamespaceComponent.extensions);
        it["generates matchers for each pool"] = () => generates<CComponent>(CComponent.extensions);
        it["generates custom prefix"] = () => generates<CustomPrefixComponent>(CustomPrefixComponent.extensions);
        it["generates component with default pool"] = () => generates<DefaultPoolComponent>(DefaultPoolComponent.extensions);
        it["generates component with default pool and others"] = () => generates<MultiplePoolAndDefaultPoolComponent>(MultiplePoolAndDefaultPoolComponent.extensions);

        it["generates component for class"] = () => generates<SomeClass>(SomeClass.extensions);
        it["generates component for struct"] = () => generates<SomeStruct>(SomeStruct.extensions);
    }
}

