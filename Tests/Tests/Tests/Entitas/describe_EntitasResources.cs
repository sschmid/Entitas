using Entitas;
using NSpec;

class describe_EntitasResources : nspec {

    void when_version() {

        it["gets version"] = () => {
            EntitasResources.GetVersion().should_not_be_null();
        };
    }
}
