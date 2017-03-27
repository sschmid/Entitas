using Entitas;
using NSpec;

class describe_EntitasVersion : nspec {

    void when_version() {

        xit["gets version"] = () => {
            EntitasVersion.GetVersion().should_be("0.40.0");
        };
    }
}
