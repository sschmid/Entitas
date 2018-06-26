using System;
using Entitas;
using Entitas.CodeGeneration.Plugins;
using Entitas.Utils;
using My.Namespace;
using NSpec;

class describe_ContextComponentDataProvider : nspec {

    ComponentData getData<T>(Properties properties = null) {
        return getMultipleData<T>(properties)[0];
    }

    ComponentData[] getMultipleData<T>(Properties properties = null) {
        var provider = new ComponentDataProvider(new Type[] { typeof(T) });
        if (properties == null) {
            properties = new Properties("Entitas.CodeGeneration.Plugins.Contexts = Game, GameState");
        }
        provider.Configure(properties);

        return (ComponentData[])provider.GetData();
    }

    void when_providing() {

        context["context names"] = () => {
            
            it["when no contexts are specified"] = () => {
                var data = getData<NoContextComponent>();

                data.GetContextNames().Length.should_be(1);
            };
            
            it["when contexts are specified"] = () => {
                var data = getData<NameAgeComponent>();

                data.GetContextNames().Length.should_be(2);
                data.GetContextNames().should_contain("Test");
                data.GetContextNames().should_contain("Test2");
            };
            
            context["fail"] = () => {
                it["when there are duplicate context attributes specified"] = () => {                
                    expect<EntitasException>(() => getData<ComponentWithDuplicatedContexts>());
                };
            };
        };
    }
}
