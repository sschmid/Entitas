using Entitas.CodeGenerator;
using NSpec;

class describe_ContextDataProvider : nspec {

    void when_providing() {

        it["creates data for each context name"] = () => {
            var names = new [] { "Input", "GameState" };
            var provider = new ContextDataProvider(names);

            var data = (ContextData[])provider.GetData();

            data.Length.should_be(names.Length);
            data[0].GetContextName().should_be(names[0]);
            data[1].GetContextName().should_be(names[1]);
        };
    }
}
