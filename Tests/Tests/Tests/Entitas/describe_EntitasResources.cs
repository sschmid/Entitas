using Entitas;
using NSpec;
using Shouldly;

class describe_EntitasResources : nspec {

    void when_version() {

        it["gets version"] = () => {
            EntitasResources.GetVersion().ShouldNotBeNull();
        };
    }
}
