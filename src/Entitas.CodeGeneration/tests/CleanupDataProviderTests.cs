using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Roslyn;
using DesperateDevs.Serialization;
using Entitas.CodeGeneration.Attributes;
using Microsoft.CodeAnalysis;
using Entitas.CodeGeneration.Plugins;
using Entitas.Roslyn.CodeGeneration.Plugins;
using FluentAssertions;
using Xunit;

namespace Entitas.CodeGeneration.Tests
{
    public class CleanupDataProviderTests
    {
        static readonly string ProjectRoot = TestExtensions.GetProjectRoot();
        static readonly string ProjectPath = $"{ProjectRoot}/Tests/TestFixtures/TestFixtures.csproj";

        INamedTypeSymbol[] Types => _types ??= new ProjectParser(ProjectPath).GetTypes();
        INamedTypeSymbol[] _types;

        readonly CleanupData _dataRemove;
        readonly CleanupData _dataDestroy;

        public CleanupDataProviderTests()
        {
            _dataRemove = GetData<CleanupRemoveComponent>();
            _dataDestroy = GetData<CleanupDestroyComponent>();
        }

        [Fact]
        public void GetsFullTypeName()
        {
            _dataRemove.componentData.GetTypeName().Should().Be(typeof(CleanupRemoveComponent).ToCompilableString());
            _dataDestroy.componentData.GetTypeName().Should().Be(typeof(CleanupDestroyComponent).ToCompilableString());
        }

        [Fact]
        public void GetsMode()
        {
            _dataRemove.cleanupMode.Should().Be(CleanupMode.RemoveComponent);
            _dataDestroy.cleanupMode.Should().Be(CleanupMode.DestroyEntity);
        }

        [Fact]
        public void CreatesDataForEachType()
        {
            var symbol1 = Types.Single(c => c.ToCompilableString() == typeof(CleanupRemoveComponent).FullName);
            var symbol2 = Types.Single(c => c.ToCompilableString() == typeof(CleanupDestroyComponent).FullName);
            var provider = new CleanupDataProvider(new[] {symbol1, symbol2});
            provider.Configure(
                new TestPreferences("Entitas.CodeGeneration.Plugins.Contexts = Game, GameState")
            );
            provider.GetData().Length.Should().Be(2);
        }

        CleanupData GetData<T>(Preferences preferences = null) => GetMultipleData<T>(preferences)[0];

        CleanupData[] GetMultipleData<T>(Preferences preferences = null)
        {
            var type = typeof(T);
            var symbol = Types.Single(c => c.ToCompilableString() == type.FullName);
            var provider = new CleanupDataProvider(new[] {symbol});
            preferences ??= new TestPreferences(
                @"Entitas.CodeGeneration.Plugins.Contexts = Game, GameState
Entitas.CodeGeneration.Plugins.IgnoreNamespaces = false");

            provider.Configure(preferences);
            return (CleanupData[])provider.GetData();
        }
    }
}
