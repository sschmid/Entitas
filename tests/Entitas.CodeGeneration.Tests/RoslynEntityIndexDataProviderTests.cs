using System.IO;
using System.Linq;
using DesperateDevs.Roslyn;
using DesperateDevs.Serialization;
using Entitas.Plugins;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using My.Namespace;
using MyNamespace;
using Xunit;

namespace Entitas.CodeGeneration.Tests
{
    [Collection("Entitas.CodeGeneration.Tests")]
    public class RoslynEntityIndexDataProviderTests
    {
        static readonly string ProjectRoot = TestExtensions.GetProjectRoot();
        static readonly string ProjectPath = Path.Combine(ProjectRoot, "tests", "Fixtures", "Fixtures.csproj");

        INamedTypeSymbol[] Types => _types ??= new ProjectParser(ProjectPath).GetTypes();
        INamedTypeSymbol[] _types;

        [Fact]
        public void CreatesDataForSingleEntityIndex()
        {
            var data = GetData<EntityIndexComponent, StandardComponent>();
            data.Length.Should().Be(1);
            var d = data[0];
            d.Type.Should().Be("Entitas.EntityIndex");
            d.IsCustom.Should().BeFalse();
            d.Name.Should().Be("MyNamespaceEntityIndex");
            d.Contexts.Length.Should().Be(2);
            d.Contexts[0].Should().Be("Test1");
            d.Contexts[1].Should().Be("Test2");
            d.KeyType.Should().Be("string");
            d.ComponentType.Should().Be("My.Namespace.EntityIndexComponent");
            d.MemberName.Should().Be("value");
        }

        [Fact]
        public void CreatesDataForMultipleEntityIndexes()
        {
            var data = GetData<MultipleEntityIndicesComponent, StandardComponent>();
            data.Length.Should().Be(2);
            data[0].Name.Should().Be("MyNamespaceMultipleEntityIndices");
            data[0].HasMultiple.Should().BeTrue();
            data[1].Name.Should().Be("MyNamespaceMultipleEntityIndices");
            data[1].HasMultiple.Should().BeTrue();
        }

        [Fact]
        public void CreatesDataForSinglePrimaryEntityIndex()
        {
            var data = GetData<PrimaryEntityIndexComponent, StandardComponent>();
            data.Length.Should().Be(1);
            var d = data[0];
            d.Type.Should().Be("Entitas.PrimaryEntityIndex");
            d.IsCustom.Should().BeFalse();
            d.Name.Should().Be("PrimaryEntityIndex");
            d.Contexts.Length.Should().Be(1);
            d.Contexts[0].Should().Be("Game");
            d.KeyType.Should().Be("string");
            d.ComponentType.Should().Be("PrimaryEntityIndexComponent");
            d.MemberName.Should().Be("value");
        }

        [Fact]
        public void CreatesDataForMultiplePrimaryEntityIndexes()
        {
            var data = GetData<MultiplePrimaryEntityIndicesComponent, StandardComponent>();
            data.Length.Should().Be(2);
            data[0].Name.Should().Be("MultiplePrimaryEntityIndices");
            data[0].HasMultiple.Should().BeTrue();
            data[1].Name.Should().Be("MultiplePrimaryEntityIndices");
            data[1].HasMultiple.Should().BeTrue();
        }

        [Fact]
        public void IgnoresAbstractComponents()
        {
            GetData<AbstractEntityIndexComponent, StandardComponent>().Length.Should().Be(0);
        }

        [Fact]
        public void CreatesDataForCustomEntityIndexClass()
        {
            var data = GetData<CustomEntityIndex, StandardComponent>();
            data.Length.Should().Be(1);
            var d = data[0];
            d.Type.Should().Be("MyNamespace.CustomEntityIndex");
            d.IsCustom.Should().BeTrue();
            d.Name.Should().Be("MyNamespaceCustomEntityIndex");
            d.Contexts.Length.Should().Be(1);
            d.Contexts[0].Should().Be("Test1");
            d.CustomMethods.Length.Should().Be(2);
        }

        [Fact]
        public void IgnoresNonIComponent()
        {
            GetData<ClassWithEntitIndexAttribute, StandardComponent>().Length.Should().Be(0);
        }

        [Fact]
        public void IgnoresNamespaces()
        {
            var preferences = new TestPreferences(
                "Entitas.CodeGeneration.Plugins.Contexts = ConfiguredContext" + "\n" +
                "Entitas.CodeGeneration.Plugins.IgnoreNamespaces = true"
            );
            var data = GetData<EntityIndexComponent, StandardComponent>(preferences);
            data.Length.Should().Be(1);
            var d = data[0];
            d.Name.Should().Be("EntityIndex");
        }

        [Fact]
        public void GetsDefaultContext()
        {
            var preferences = new TestPreferences(
                "Entitas.CodeGeneration.Plugins.Contexts = ConfiguredContext" + "\n" +
                "Entitas.CodeGeneration.Plugins.IgnoreNamespaces = true"
            );
            var data = GetData<EntityIndexNoContextComponent, StandardComponent>(preferences);
            data.Length.Should().Be(1);
            var d = data[0];
            d.Contexts.Length.Should().Be(1);
            d.Contexts[0].Should().Be("ConfiguredContext");
        }

        INamedTypeSymbol GetSymbol<T>() => Types.Single(c => c.ToCompilableString() == typeof(T).FullName);

        EntityIndexData[] GetData<T1, T2>(Preferences preferences = null)
        {
            var symbols = new[] {GetSymbol<T1>(), GetSymbol<T2>()};
            var provider = new Entitas.Roslyn.CodeGeneration.Plugins.EntityIndexDataProvider(symbols);
            preferences ??= new TestPreferences(
                "Entitas.CodeGeneration.Plugins.Contexts = Game, GameState" + "\n" +
                "Entitas.CodeGeneration.Plugins.IgnoreNamespaces = false"
            );

            provider.Configure(preferences);
            return (EntityIndexData[])provider.GetData();
        }
    }
}
