using System.Linq;
using DesperateDevs.Roslyn;
using DesperateDevs.Serialization;
using Entitas.CodeGeneration.Plugins;
using Microsoft.CodeAnalysis;
using My.Namespace;
using MyNamespace;
using NSpec;

class describe_RoslynEntityIndexDataProvider : nspec {

    static string projectRoot = TestExtensions.GetProjectRoot();
    static string projectPath = projectRoot + "/Tests/TestFixtures/TestFixtures.csproj";

    INamedTypeSymbol[] types {
        get {
            if(_types == null) {
                var parser = new ProjectParser(projectPath);
                _types = parser.GetTypes();
            }

            return _types;
        }
    }

    INamedTypeSymbol[] _types;

    INamedTypeSymbol getSymbol<T>() {
        var type = typeof(T);
        return types.Single(c => c.ToCompilableString() == type.FullName);
    }

    EntityIndexData[] getData<T1, T2>(Preferences preferences = null) {
        var symbols = new [] { getSymbol<T1>(), getSymbol<T2>() };
        var provider = new Entitas.Roslyn.CodeGeneration.Plugins.EntityIndexDataProvider(symbols);
        if(preferences== null) {
            preferences= new TestPreferences(
                "Entitas.CodeGeneration.Plugins.Contexts = Game, GameState" + "\n" +
                "Entitas.CodeGeneration.Plugins.IgnoreNamespaces = false"
            );
        }
        provider.Configure(preferences);

        return (EntityIndexData[])provider.GetData();
    }

    void when_providing() {

        it["creates data for single entity index"] = () => {
            var data = getData<EntityIndexComponent, StandardComponent>();
            data.Length.should_be(1);

            var d = data[0];

            d.GetEntityIndexType().should_be("Entitas.EntityIndex");
            d.IsCustom().should_be_false();
            d.GetEntityIndexName().should_be("MyNamespaceEntityIndex");
            d.GetContextNames().Length.should_be(2);
            d.GetContextNames()[0].should_be("Test");
            d.GetContextNames()[1].should_be("Test2");
            d.GetKeyType().should_be("string");
            d.GetComponentType().should_be("My.Namespace.EntityIndexComponent");
            d.GetMemberName().should_be("value");
        };

        it["creates data for multiple entity index"] = () => {
            var data = getData<MultipleEntityIndicesComponent, StandardComponent>();
            data.Length.should_be(2);

            data[0].GetEntityIndexName().should_be("MyNamespaceMultipleEntityIndices");
            data[0].GetHasMultiple().should_be_true();

            data[1].GetEntityIndexName().should_be("MyNamespaceMultipleEntityIndices");
            data[1].GetHasMultiple().should_be_true();
        };

        it["creates data for single primary entity index"] = () => {
            var data = getData<PrimaryEntityIndexComponent, StandardComponent>();
            data.Length.should_be(1);

            var d = data[0];

            d.GetEntityIndexType().should_be("Entitas.PrimaryEntityIndex");
            d.IsCustom().should_be_false();
            d.GetEntityIndexName().should_be("PrimaryEntityIndex");
            d.GetContextNames().Length.should_be(1);
            d.GetContextNames()[0].should_be("Game");
            d.GetKeyType().should_be("string");
            d.GetComponentType().should_be("PrimaryEntityIndexComponent");
            d.GetMemberName().should_be("value");
        };

        it["creates data for multiple primary entity index"] = () => {
            var data = getData<MultiplePrimaryEntityIndicesComponent, StandardComponent>();
            data.Length.should_be(2);

            data[0].GetEntityIndexName().should_be("MultiplePrimaryEntityIndices");
            data[0].GetHasMultiple().should_be_true();

            data[1].GetEntityIndexName().should_be("MultiplePrimaryEntityIndices");
            data[1].GetHasMultiple().should_be_true();
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

            Preferences preferences = null;

            before = () => {
                preferences = new TestPreferences(
                    "Entitas.CodeGeneration.Plugins.Contexts = ConfiguredContext" + "\n" +
                    "Entitas.CodeGeneration.Plugins.IgnoreNamespaces = true"
                );
            };

            it["ignores namespaces"] = () => {
                var data = getData<EntityIndexComponent, StandardComponent>(preferences);
                data.Length.should_be(1);
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
