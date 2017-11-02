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

        it["creates data with kits"] = () => {
            var names = "Entitas.CodeGeneration.Plugins.Contexts = Input: GestureKit, Game: PhysicsKit + PathFindingKit";
            var provider = new ContextDataProvider();
            provider.Configure(new Preferences(new Properties(names)));

            var data = (ContextData[])provider.GetData();
            data.Length.should_be(2);

            data[0].GetContextName().should_be("Input");
            data[0].GetKits().Length.should_be(1);
            var inputKits = data[0].GetKits();
            inputKits[0].should_be("GestureKit");

            data[1].GetContextName().should_be("Game");
            data[1].GetKits().Length.should_be(2);
            var gameKits = data[1].GetKits();
            gameKits[0].should_be("PhysicsKit");
            gameKits[1].should_be("PathFindingKit");
        };
    }
}
