using Entitas;
using NSpec;

class describe_EntityStringExtension : nspec {

    const string ENTITY_SUFFIX = "Entity";

    void when_entity() {

        context["when adding EntitySuffix"] = () => {

            it["doesn't add entity suffix to string ending with EntitySuffix"] = () => {
                const string str = "Game" + ENTITY_SUFFIX;
                str.AddEntitySuffix().should_be_same(str);
            };

            it["adds EntitySuffix to string not ending with EntitySuffix"] = () => {
                const string str = "Game";
                str.AddEntitySuffix().should_be("Game" + ENTITY_SUFFIX);
            };
        };

        context["when removing EntitySuffix"] = () => {

            it["doesn't change string when not ending with EntitySuffix"] = () => {
                const string str = "Game";
                str.RemoveEntitySuffix().should_be_same(str);
            };

            it["removes EntitySuffix when ending with EntitySuffix"] = () => {
                const string str = "Game" + ENTITY_SUFFIX;
                str.RemoveEntitySuffix().should_be("Game");
            };
        };
    }
}
