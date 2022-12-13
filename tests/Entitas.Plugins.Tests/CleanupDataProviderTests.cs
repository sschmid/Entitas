using System.IO;
using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Roslyn;
using DesperateDevs.Serialization;
using Entitas.Plugins.Attributes;
using Microsoft.CodeAnalysis;
using FluentAssertions;
using Xunit;

namespace Entitas.Plugins.Tests
{
    [Collection("Entitas.Plugins.Tests")]
    public class CleanupDataProviderTests
    {
        static readonly string ProjectRoot = TestExtensions.GetProjectRoot();
        static readonly string ProjectPath = Path.Combine(ProjectRoot, "tests", "Fixtures", "Fixtures.csproj");

        static INamedTypeSymbol[] Types => _types ??= new ProjectParser(ProjectPath).GetTypes();
        static INamedTypeSymbol[] _types;

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
            _dataRemove.ComponentData.Type.Should().Be(typeof(CleanupRemoveComponent).ToCompilableString());
            _dataDestroy.ComponentData.Type.Should().Be(typeof(CleanupDestroyComponent).ToCompilableString());
        }

        [Fact]
        public void GetsMode()
        {
            _dataRemove.CleanupMode.Should().Be(CleanupMode.RemoveComponent);
            _dataDestroy.CleanupMode.Should().Be(CleanupMode.DestroyEntity);
        }

        [Fact]
        public void CreatesDataForEachType()
        {
            var symbol1 = Types.Single(c => c.ToCompilableString() == typeof(CleanupRemoveComponent).FullName);
            var symbol2 = Types.Single(c => c.ToCompilableString() == typeof(CleanupDestroyComponent).FullName);
            var provider = new CleanupDataProvider(new[] {symbol1, symbol2});
            provider.Configure(
                new TestPreferences("Entitas.Plugins.Contexts = Game, GameState")
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
                @"Entitas.Plugins.Contexts = Game, GameState
Entitas.Plugins.IgnoreNamespaces = false");

            provider.Configure(preferences);
            return (CleanupData[])provider.GetData();
        }
    }
}
