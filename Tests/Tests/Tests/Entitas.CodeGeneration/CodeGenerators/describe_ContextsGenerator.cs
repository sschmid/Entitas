using Entitas.CodeGeneration;
using Entitas.CodeGeneration.Plugins;
using NSpec;

class describe_ContextsGenerator : nspec {

    void when_generating() {

        it["doesn't generates any usings"] = () => {
            var generator = new ContextsGenerator();
            var files = generator.Generate(new CodeGeneratorData[0]);
            files.Length.should_be(1);
            files[0].fileContent.should_start_with("public partial class");
        };
        it["generates a code using the GetType().GetMethods()"] = () => {
            var generator = new ContextsGenerator();
            var files = generator.Generate(new CodeGeneratorData[0]);
            files.Length.should_be(1);
            files[0].fileContent.should_contain("GetType().GetMethods()");
        };
        it["generates a code without using the System.Attribute.IsDefined("] = () => {
            var generator = new ContextsGenerator();
            var files = generator.Generate(new CodeGeneratorData[0]);
            files.Length.should_be(1);
            files[0].fileContent.should_contain("System.Attribute.IsDefined");
        };
    }
    void when_instantiated() {
        it["should be marked as enabled by default"] = () => {
            var generator = new ContextsGenerator();
            generator.isEnabledByDefault.should_be_true();
        };
    }
}