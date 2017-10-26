using Entitas.CodeGeneration;
using Entitas.CodeGeneration.Plugins;
using NSpec;

class describe_ContextsPCLGenerator : nspec {

    void when_generating() {

        it["generates the required usings"] = () => {
            var generator = new ContextsPCLGenerator();
            var files = generator.Generate(new CodeGeneratorData[0]);
            files.Length.should_be(1);
            files[0].fileContent.should_start_with(ContextsPCLGenerator.CONTEXT_USING);
        };
        it["generates a code without including the GetType().GetMethods()"] = () => {
            var generator = new ContextsPCLGenerator();
            var files = generator.Generate(new CodeGeneratorData[0]);
            files.Length.should_be(1);
            files[0].fileContent.should_not_match("GetType().GetMethods()");
        };
        it["generates a code without including the System.Attribute.IsDefined("] = () => {
            var generator = new ContextsPCLGenerator();
            var files = generator.Generate(new CodeGeneratorData[0]);
            files.Length.should_be(1);
            files[0].fileContent.should_not_match("System.Attribute.IsDefined");
        };
        it["generates a code using the GetType().GetRuntimeMethods()"] = () => {
            var generator = new ContextsPCLGenerator();
            var files = generator.Generate(new CodeGeneratorData[0]);
            files.Length.should_be(1);
            files[0].fileContent.should_contain("GetType().GetRuntimeMethods()");
        };
        it["generates a code using the GetCustomAttribute"] = () => {
            var generator = new ContextsPCLGenerator();
            var files = generator.Generate(new CodeGeneratorData[0]);
            files.Length.should_be(1);
            files[0].fileContent.should_contain("method.GetCustomAttribute");
        };
    }
    void when_instantiated() {
        it["shouldn't be marked as enabled by default"] = () => {
            var generator = new ContextsPCLGenerator();
            generator.isEnabledByDefault.should_be_false();
        };
    }
}
