using System;
using Entitas.CodeGeneration.Plugins;
using Entitas.Utils;
using My.Namespace;
using MyNamespace;
using NSpec;

class describe_EntityIndexDataProvider : nspec {

    EntityIndexData[] getData<T1, T2>(Preferences preferences= null) {
        var provider = new EntityIndexDataProvider(new Type[] { typeof(T1), typeof(T2) });
        if (preferences == null) {
            preferences = new Preferences(new Properties(
                "Entitas.CodeGeneration.Plugins.Contexts = Game, GameState" + "\n" +
                "Entitas.CodeGeneration.Plugins.IgnoreNamespaces = false"
            ));
        }
        provider.Configure(preferences);

        return (EntityIndexData[])provider.GetData();
    }

    void when_providing() {

        it["creates data for each entity index"] = () => {
            var data = getData<EntityIndexComponent, StandardComponent>();
            data.Length.should_be(2);

            var d = data[0];

            d.GetEntityIndexType().GetType().should_be(typeof(string));
            d.GetEntityIndexType().should_be("Entitas.EntityIndex");

            d.IsCustom().GetType().should_be(typeof(bool));
            d.IsCustom().should_be_false();

            d.GetEntityIndexName().GetType().should_be(typeof(string));
            d.GetEntityIndexName().should_be("MyNamespaceEntityIndex");

            d.GetContextNames().GetType().should_be(typeof(string[]));
            d.GetContextNames().Length.should_be(2);
            d.GetContextNames()[0].should_be("Test");
            d.GetContextNames()[1].should_be("Test2");

            d.GetKeyType().GetType().should_be(typeof(string));
            d.GetKeyType().should_be("string");

            d.GetComponentType().GetType().should_be(typeof(string));
            d.GetComponentType().should_be("My.Namespace.EntityIndexComponent");

            d.GetMemberName().GetType().should_be(typeof(string));
            d.GetMemberName().should_be("value");

            data[1].GetMemberName().should_be("value2");
        };

        it["creates data for each primary entity index"] = () => {
            var data = getData<PrimaryEntityIndexComponent, StandardComponent>();

            data.Length.should_be(2);
            var d = data[0];

            d.GetEntityIndexType().should_be("Entitas.PrimaryEntityIndex");
            d.IsCustom().should_be_false();
            d.GetEntityIndexName().should_be("PrimaryEntityIndex");
            d.GetContextNames().Length.should_be(1);
            d.GetContextNames()[0].should_be("Game");
            d.GetKeyType().should_be("string");
            d.GetComponentType().should_be("PrimaryEntityIndexComponent");
            d.GetMemberName().should_be("value");

            data[1].GetMemberName().should_be("value2");
        };

        it["ignores abstract components"] = () => {
            var data = getData<AbstractEntityIndexComponent, StandardComponent>();
            data.Length.should_be(0);
        };

        it["creates data for custom entity index class"] = () => {
            var data = getData<CustomEntityIndex, StandardComponent>();

            data.Length.should_be(1);
            var d = data[0];

            d.GetEntityIndexType().should_be("MyNamespace.CustomEntityIndex");
            d.IsCustom().should_be_true();
            d.GetEntityIndexName().should_be("MyNamespaceCustomEntityIndex");
            d.GetContextNames().Length.should_be(1);
            d.GetContextNames()[0].should_be("Test");

            var methods = d.GetCustomMethods();
            methods.GetType().should_be(typeof(MethodData[]));
            methods.Length.should_be(2);
        };

        it["ignores non IComponent"] = () => {
            var data = getData<ClassWithEntitIndexAttribute, StandardComponent>();
            data.Length.should_be(0);
        };

        context["configure"] = () => {

            Preferences preferences= null;

            before = () => {
                preferences = new Preferences(new Properties(
                    "Entitas.CodeGeneration.Plugins.Contexts = ConfiguredContext" + "\n" +
                    "Entitas.CodeGeneration.Plugins.IgnoreNamespaces = true"
                ));
            };

            it["ignores namespaces"] = () => {
                var data = getData<EntityIndexComponent, StandardComponent>(preferences);
                data.Length.should_be(2);
                var d = data[0];

                d.GetEntityIndexName().should_be("EntityIndex");

            };

            it["gets default context"] = () => {
                var data = getData<EntityIndexNoContextComponent, StandardComponent>(preferences);

                data.Length.should_be(1);
                var d = data[0];

                d.GetContextNames().Length.should_be(1);
                d.GetContextNames()[0].should_be("ConfiguredContext");
            };
        };
    }
}
