using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Roslyn;
using DesperateDevs.Serialization;
using Entitas.CodeGeneration.Attributes;
using Microsoft.CodeAnalysis;
using NSpec;
using Entitas.CodeGeneration.Plugins;
using Entitas.Roslyn.CodeGeneration.Plugins;

class describe_CleanupDataProvider : nspec {

    static string projectRoot = TestExtensions.GetProjectRoot();
    static string projectPath = projectRoot + "/Tests/TestFixtures/TestFixtures.csproj";

    INamedTypeSymbol[] types {
        get {
            if (_types == null) {
                var parser = new ProjectParser(projectPath);
                _types = parser.GetTypes();
            }

            return _types;
        }
    }

    INamedTypeSymbol[] _types;

    CleanupData getData<T>(Preferences preferences = null) {
        return getMultipleData<T>(preferences)[0];
    }

    CleanupData[] getMultipleData<T>(Preferences preferences = null) {
        var type = typeof(T);
        var symbol = types.Single(c => c.ToCompilableString() == type.FullName);
        var provider = new Entitas.Roslyn.CodeGeneration.Plugins.CleanupDataProvider(new[] { symbol });
        if (preferences == null) {
            preferences = new TestPreferences(
                @"Entitas.CodeGeneration.Plugins.Contexts = Game, GameState
Entitas.CodeGeneration.Plugins.IgnoreNamespaces = false");
        }

        provider.Configure(preferences);

        return (CleanupData[])provider.GetData();
    }

    void when_providing() {

        context["cleanup remove"] = () => {

            CleanupData dataRemove = null;
            CleanupData dataDestroy = null;

            before = () => {
                dataRemove = getData<CleanupRemoveComponent>();
                dataDestroy = getData<CleanupDestroyComponent>();
            };

            it["gets full type name"] = () => {
                dataRemove.componentData.GetTypeName().should_be(typeof(CleanupRemoveComponent).ToCompilableString());
                dataDestroy.componentData.GetTypeName().should_be(typeof(CleanupDestroyComponent).ToCompilableString());
            };

            it["gets mode"] = () => {
                dataRemove.cleanupMode.should_be(CleanupMode.RemoveComponent);
                dataDestroy.cleanupMode.should_be(CleanupMode.DestroyEntity);
            };
        };

        context["multiple types"] = () => {

            it["creates data for each type"] = () => {
                var symbol1 = types.Single(c => c.ToCompilableString() == typeof(CleanupRemoveComponent).FullName);
                var symbol2 = types.Single(c => c.ToCompilableString() == typeof(CleanupDestroyComponent).FullName);
                var provider = new Entitas.Roslyn.CodeGeneration.Plugins.CleanupDataProvider(new[] { symbol1, symbol2 });
                provider.Configure(
                    new TestPreferences("Entitas.CodeGeneration.Plugins.Contexts = Game, GameState")
                );
                var data = provider.GetData();
                data.Length.should_be(2);
            };
        };
    }
}
