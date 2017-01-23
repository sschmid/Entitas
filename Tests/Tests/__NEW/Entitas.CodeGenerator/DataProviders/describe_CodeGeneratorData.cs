using Entitas.CodeGenerator;
using NSpec;

class describe_CodeGeneratorData : nspec {

    void when_providing() {

        it["creates data with dataProviderName"] = () => {
            const string dataProviderName = "Some Generator";
            var data = new CodeGeneratorData(dataProviderName);
            data.dataProviderName.should_be(dataProviderName);
        };

        it["contains added object"] = () => {
            var data = new CodeGeneratorData("");
            data["key"] = "value";
            data["key"].should_be("value");
        };
    }
}
