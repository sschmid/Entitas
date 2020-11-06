using Entitas.CodeGeneration.Plugins;
using NSpec;
using Shouldly;

class describe_ContextDataProvider : nspec {

    void when_providing() {

        it["creates data for each context name"] = () => {
            var names = "Entitas.CodeGeneration.Plugins.Contexts = Input, GameState";
            var provider = new ContextDataProvider();
            provider.Configure(new TestPreferences(names));

            var data = (ContextData[])provider.GetData();

            data.Length.ShouldBe(2);
            data[0].GetContextName().ShouldBe("Input");
            data[1].GetContextName().ShouldBe("GameState");
        };
    }
}
