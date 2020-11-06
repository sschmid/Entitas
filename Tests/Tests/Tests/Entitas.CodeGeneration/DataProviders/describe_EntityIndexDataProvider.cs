using System;
using DesperateDevs.Serialization;
using Entitas.CodeGeneration.Plugins;
using My.Namespace;
using MyNamespace;
using NSpec;
using Shouldly;

class describe_EntityIndexDataProvider : nspec {

    EntityIndexData[] getData<T1, T2>(Preferences preferences= null) {
        var provider = new EntityIndexDataProvider(new Type[] { typeof(T1), typeof(T2) });
        if (preferences == null) {
            preferences = new TestPreferences("Entitas.CodeGeneration.Plugins.Contexts = Game, GameState");
        }
        provider.Configure(preferences);

        return (EntityIndexData[])provider.GetData();
    }

    void when_providing() {

        it["creates data for single entity index"] = () => {
            var data = getData<EntityIndexComponent, StandardComponent>();
            data.Length.ShouldBe(1);

            var d = data[0];

            d.GetEntityIndexType().GetType().ShouldBe(typeof(string));
            d.GetEntityIndexType().ShouldBe("Entitas.EntityIndex");

            d.IsCustom().GetType().ShouldBe(typeof(bool));
            d.IsCustom().ShouldBeFalse();

            d.GetEntityIndexName().GetType().ShouldBe(typeof(string));
            d.GetEntityIndexName().ShouldBe("EntityIndex");

            d.GetContextNames().GetType().ShouldBe(typeof(string[]));
            d.GetContextNames().Length.ShouldBe(2);
            d.GetContextNames()[0].ShouldBe("Test");
            d.GetContextNames()[1].ShouldBe("Test2");

            d.GetKeyType().GetType().ShouldBe(typeof(string));
            d.GetKeyType().ShouldBe("string");

            d.GetComponentType().GetType().ShouldBe(typeof(string));
            d.GetComponentType().ShouldBe("My.Namespace.EntityIndexComponent");

            d.GetMemberName().GetType().ShouldBe(typeof(string));
            d.GetMemberName().ShouldBe("value");

            d.GetHasMultiple().GetType().ShouldBe(typeof(bool));
            d.GetHasMultiple().ShouldBeFalse();
        };

        it["creates data for multiple entity index"] = () => {
            var data = getData<MultipleEntityIndicesComponent, StandardComponent>();
            data.Length.ShouldBe(2);

            data[0].GetEntityIndexName().ShouldBe("MultipleEntityIndices");
            data[0].GetHasMultiple().ShouldBeTrue();

            data[1].GetEntityIndexName().ShouldBe("MultipleEntityIndices");
            data[1].GetHasMultiple().ShouldBeTrue();
        };

        it["creates data for single primary entity index"] = () => {
            var data = getData<PrimaryEntityIndexComponent, StandardComponent>();

            data.Length.ShouldBe(1);
            var d = data[0];

            d.GetEntityIndexType().ShouldBe("Entitas.PrimaryEntityIndex");
            d.IsCustom().ShouldBeFalse();
            d.GetEntityIndexName().ShouldBe("PrimaryEntityIndex");
            d.GetContextNames().Length.ShouldBe(1);
            d.GetContextNames()[0].ShouldBe("Game");
            d.GetKeyType().ShouldBe("string");
            d.GetComponentType().ShouldBe("PrimaryEntityIndexComponent");
            d.GetMemberName().ShouldBe("value");
            d.GetHasMultiple().ShouldBeFalse();
        };

        it["creates data for multiple primary entity index"] = () => {
            var data = getData<MultiplePrimaryEntityIndicesComponent, StandardComponent>();

            data.Length.ShouldBe(2);

            data[0].GetEntityIndexName().ShouldBe("MultiplePrimaryEntityIndices");
            data[0].GetHasMultiple().ShouldBeTrue();

            data[1].GetEntityIndexName().ShouldBe("MultiplePrimaryEntityIndices");
            data[1].GetHasMultiple().ShouldBeTrue();
        };

        it["ignores abstract components"] = () => {
            var data = getData<AbstractEntityIndexComponent, StandardComponent>();
            data.Length.ShouldBe(0);
        };

        it["creates data for custom entity index class"] = () => {
            var data = getData<CustomEntityIndex, StandardComponent>();

            data.Length.ShouldBe(1);
            var d = data[0];

            d.GetEntityIndexType().ShouldBe("MyNamespace.CustomEntityIndex");
            d.IsCustom().ShouldBeTrue();
            d.GetEntityIndexName().ShouldBe("MyNamespaceCustomEntityIndex");
            d.GetContextNames().Length.ShouldBe(1);
            d.GetContextNames()[0].ShouldBe("Test");

            var methods = d.GetCustomMethods();
            methods.GetType().ShouldBe(typeof(MethodData[]));
            methods.Length.ShouldBe(2);
        };

        it["ignores non IComponent"] = () => {
            var data = getData<ClassWithEntitIndexAttribute, StandardComponent>();
            data.Length.ShouldBe(0);
        };

        context["configure"] = () => {

            Preferences preferences= null;

            before = () => {
                preferences = new TestPreferences("Entitas.CodeGeneration.Plugins.Contexts = ConfiguredContext");
            };

            it["gets default context"] = () => {
                var data = getData<EntityIndexNoContextComponent, StandardComponent>(preferences);

                data.Length.ShouldBe(1);
                var d = data[0];

                d.GetContextNames().Length.ShouldBe(1);
                d.GetContextNames()[0].ShouldBe("ConfiguredContext");
            };
        };
    }
}
