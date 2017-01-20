using NSpec;
using Entitas.CodeGenerator;

class describe_BlueprintDataProvider : nspec {

    void when_providing() {

        it["creates data for each blueprint name"] = () => {
            var names = new[] { "Player", "Ship" };
            var provider = new BlueprintDataProvider(names);

            var data = provider.GetData();

            data.Length.should_be(names.Length);
            data[0].GetBlueprintName().should_be(names[0]);
            data[1].GetBlueprintName().should_be(names[1]);
        };

        it["sortes data by blueprint name"] = () => {
            var names = new[] { "Ship", "Player" };
            var provider = new BlueprintDataProvider(names);

            var data = provider.GetData();

            data[0].GetBlueprintName().should_be(names[1]);
            data[1].GetBlueprintName().should_be(names[0]);
        };
    }
}
