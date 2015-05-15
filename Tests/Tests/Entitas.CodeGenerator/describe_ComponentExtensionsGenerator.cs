using NSpec;
using Entitas.CodeGenerator;
using System;
using My.Namespace;

class describe_ComponentExtensionsGenerator : nspec {

    bool logResults = false;

    const string classSuffix = "GeneratedExtension";

    void generates(Type type, string code) {
        var extensions = ComponentExtensionsGenerator
            .GenerateComponentExtensions(new[] { type }, classSuffix);
        var filePath = type + classSuffix;

        extensions.Count.should_be(1);
        extensions.ContainsKey(filePath).should_be_true();
        if (logResults) {
            Console.WriteLine("should:\n" + code);
            Console.WriteLine("was:\n" + extensions[filePath]);
        }
        extensions[filePath].should_be(code);
    }

    void when_generating() {
        it["component without fields"] = () => generates(typeof(MovableComponent), MovableComponent.extensions);
        it["component with fields"] = () => generates(typeof(PersonComponent), PersonComponent.extensions);
        it["single singleton component"] = () => generates(typeof(AnimatingComponent), AnimatingComponent.extensions);
        it["single component with fields"] = () => generates(typeof(UserComponent), UserComponent.extensions);
        it["component for custom pool"] = () => generates(typeof(OtherPoolComponent), OtherPoolComponent.extensions);
        it["ignores [DontGenerate]"] = () => {
            var type = typeof(DontGenerateComponent);
            var extensions = ComponentExtensionsGenerator
                .GenerateComponentExtensions(new[] { type }, classSuffix);
            extensions.Count.should_be(0);
        };

        it["works with namespaces"] = () => generates(typeof(NamespaceComponent), NamespaceComponent.extensions);
    }
}

