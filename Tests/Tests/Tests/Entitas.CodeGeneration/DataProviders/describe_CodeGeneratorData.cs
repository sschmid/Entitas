using Entitas.CodeGeneration;
using NSpec;

class describe_CodeGeneratorData : nspec {

    void when_providing() {

        it["contains added object"] = () => {
            var data = new CodeGeneratorData();
            data["key"] = "value";
            data["key"].should_be("value");
        };
    }
}
