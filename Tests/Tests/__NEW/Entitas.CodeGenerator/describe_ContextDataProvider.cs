using Entitas.CodeGenerator;
using NSpec;

class describe_ContextDataProvider : nspec {

    void when_providing() {

        it["creates data for each context name"] = () => {
            var names = new[] { "GameState", "Input" };
            var provider = new ContextDataProvider(names);

            var data = provider.GetData();

            data.Length.should_be(names.Length);
            data[0].GetContextName().should_be(names[0]);
            data[1].GetContextName().should_be(names[1]);
        };

        it["sortes data by context name"] = () => {
            var names = new[] { "Input", "GameState" };
            var provider = new ContextDataProvider(names);

            var data = provider.GetData();

            data.Length.should_be(names.Length);
            data[0].GetContextName().should_be(names[1]);
            data[1].GetContextName().should_be(names[0]);
        };

        it["uppercase first letter"] = () => {
            var names = new[] { "gameState" };
            var provider = new ContextDataProvider(names);

            provider.GetData()[0].GetContextName().should_be("GameState");
        };

        it["removes duplicates"] = () => {
            var names = new[] { "gameState", "GameState", "Input" };
            var provider = new ContextDataProvider(names);

            var data = provider.GetData();

            data.Length.should_be(2);
            data[0].GetContextName().should_be("GameState");
            data[1].GetContextName().should_be("Input");
        };
    }
}
