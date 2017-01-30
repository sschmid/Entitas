using NSpec;
using Entitas.CodeGenerator;
using System.Linq;

class describe_BlueprintDataProvider : nspec {

    void when_providing() {

        it["creates data for each blueprint name"] = () => {
            var names = new[] { "Ship", "Player" };
            var provider = new BlueprintDataProvider(names);

            var data = provider.GetData().OfType<BlueprintData>().ToArray();

            data.Length.should_be(names.Length);
            data[0].GetBlueprintName().should_be(names[0]);
            data[1].GetBlueprintName().should_be(names[1]);
        };
    }
}
