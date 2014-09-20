using NSpec;
using Entitas.CodeGenerator;
using System;

class describe_ComponentExtensionsGenerator : nspec {

    bool logResults = false;

    void generates(Type type, string code) {
        var generator = new ComponentExtensionsGenerator();
        var extensions = generator.GenerateComponentExtensions(new[] { type });
        var filePath = type + ComponentExtensionsGenerator.classSuffix;

        extensions.Count.should_be(1);
        extensions.ContainsKey(filePath).should_be_true();
        if (logResults) {
            Console.WriteLine("should:\n" + code);
            Console.WriteLine("was:\n" + extensions[filePath]);
        }
        extensions[filePath].should_be(code);
    }

    void when_generating() {
        it["component without fields"] = () => generates(typeof(EmptyComponent), EmptyComponent.extensions);
        it["component with fields"] = () => generates(typeof(ArgsComponent), ArgsComponent.extensions);
        it["single singleton component"] = () => generates(typeof(SingleEmptyComponent), SingleEmptyComponent.extensions);
        it["single component with fields"] = () => generates(typeof(SingleArgsComponent), SingleArgsComponent.extensions);
        it["can handle dictionary"] = () => generates(typeof(DictionaryComponent), DictionaryComponent.extensions);
        it["ignores [DontGenerate]"] = () => {
            var type = typeof(DontGenerateComponent);
            var generator = new ComponentExtensionsGenerator();
            var extensions = generator.GenerateComponentExtensions(new[] { type });
            extensions.Count.should_be(0);
        };
    }
}

