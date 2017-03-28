using Entitas;
using NSpec;

class describe_EntitasResources : nspec {

    void when_version() {

        xit["gets version"] = () => {
            EntitasResources.GetVersion().should_be("0.40.0");
        };
    }
}
