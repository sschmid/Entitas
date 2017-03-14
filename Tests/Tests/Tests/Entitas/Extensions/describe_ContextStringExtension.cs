using Entitas;
using NSpec;

class describe_ContextStringExtension : nspec {

    const string CONTEXT_SUFFIX = "Context";

    void when_entity() {

        context["when adding ContextSuffix"] = () => {

            it["doesn't add context suffix to string ending with ContextSuffix"] = () => {
                const string str = "Game" + CONTEXT_SUFFIX;
                str.AddContextSuffix().should_be_same(str);
            };

            it["adds ContextSuffix to string not ending with ContextSuffix"] = () => {
                const string str = "Game";
                str.AddContextSuffix().should_be("Game" + CONTEXT_SUFFIX);
            };
        };

        context["when removing ContextSuffix"] = () => {

            it["doesn't change string when not ending with ContextSuffix"] = () => {
                const string str = "Game";
                str.RemoveContextSuffix().should_be_same(str);
            };

            it["removes ContextSuffix when ending with ContextSuffix"] = () => {
                const string str = "Game" + CONTEXT_SUFFIX;
                str.RemoveContextSuffix().should_be("Game");
            };
        };
    }
}
