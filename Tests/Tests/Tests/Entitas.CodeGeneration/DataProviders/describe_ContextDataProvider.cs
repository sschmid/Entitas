using Entitas.CodeGeneration.Plugins;
using NSpec;
using Entitas.Utils;

class describe_ContextDataProvider : nspec {

    void when_providing() {

        it["creates data for each context name"] = () => {
            var names = "Entitas.CodeGeneration.Plugins.Contexts = Input, GameState";
            var provider = new ContextDataProvider();
            provider.Configure(new Preferences(new Properties(names)));

            var data = (ContextData[])provider.GetData();

            data.Length.should_be(2);
            data[0].GetContextName().should_be("Input");
            data[1].GetContextName().should_be("GameState");
        };
    }
}
