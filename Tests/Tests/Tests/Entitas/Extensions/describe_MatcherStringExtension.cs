using Entitas;
using NSpec;

class describe_MatcherStringExtension : nspec {

    const string MATCHER_SUFFIX = "Matcher";

    void when_matcher() {

        context["when adding MatcherSuffix"] = () => {

            it["doesn't add entity suffix to string ending with MatcherSuffix"] = () => {
                const string str = "Game" + MATCHER_SUFFIX;
                str.AddMatcherSuffix().should_be_same(str);
            };

            it["adds MatcherSuffix to string not ending with MatcherSuffix"] = () => {
                const string str = "Game";
                str.AddMatcherSuffix().should_be("Game" + MATCHER_SUFFIX);
            };
        };

        context["when removing MatcherSuffix"] = () => {

            it["doesn't change string when not ending with MatcherSuffix"] = () => {
                const string str = "Game";
                str.RemoveMatcherSuffix().should_be_same(str);
            };

            it["removes MatcherSuffix when ending with MatcherSuffix"] = () => {
                const string str = "Game" + MATCHER_SUFFIX;
                str.RemoveMatcherSuffix().should_be("Game");
            };
        };
    }
}
