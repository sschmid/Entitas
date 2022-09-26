using System.Linq;
using DesperateDevs.Roslyn;
using DesperateDevs.Serialization;
using Entitas.CodeGeneration.Plugins;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using My.Namespace;
using MyNamespace;
using Xunit;

namespace Entitas.CodeGeneration.Tests
{
    public class RoslynEntityIndexDataProviderTests
    {
        static readonly string ProjectRoot = TestExtensions.GetProjectRoot();
        static readonly string ProjectPath = $"{ProjectRoot}/Tests/TestFixtures/TestFixtures.csproj";

        INamedTypeSymbol[] Types => _types ??= new ProjectParser(ProjectPath).GetTypes();
        INamedTypeSymbol[] _types;

        [Fact]
        public void CreatesDataForSingleEntityIndex()
        {
            var data = GetData<EntityIndexComponent, StandardComponent>();
            data.Length.Should().Be(1);
            var d = data[0];
            d.GetEntityIndexType().Should().Be("Entitas.EntityIndex");
            d.IsCustom().Should().BeFalse();
            d.GetEntityIndexName().Should().Be("MyNamespaceEntityIndex");
            d.GetContextNames().Length.Should().Be(2);
            d.GetContextNames()[0].Should().Be("Test");
            d.GetContextNames()[1].Should().Be("Test2");
            d.GetKeyType().Should().Be("string");
            d.GetComponentType().Should().Be("My.Namespace.EntityIndexComponent");
            d.GetMemberName().Should().Be("value");
        }

        [Fact]
        public void CreatesDataForMultipleEntityIndexes()
        {
            var data = GetData<MultipleEntityIndicesComponent, StandardComponent>();
            data.Length.Should().Be(2);
            data[0].GetEntityIndexName().Should().Be("MyNamespaceMultipleEntityIndices");
            data[0].GetHasMultiple().Should().BeTrue();
            data[1].GetEntityIndexName().Should().Be("MyNamespaceMultipleEntityIndices");
            data[1].GetHasMultiple().Should().BeTrue();
        }

        [Fact]
        public void CreatesDataForSinglePrimaryEntityIndex()
        {
            var data = GetData<PrimaryEntityIndexComponent, StandardComponent>();
            data.Length.Should().Be(1);
            var d = data[0];
            d.GetEntityIndexType().Should().Be("Entitas.PrimaryEntityIndex");
            d.IsCustom().Should().BeFalse();
            d.GetEntityIndexName().Should().Be("PrimaryEntityIndex");
            d.GetContextNames().Length.Should().Be(1);
            d.GetContextNames()[0].Should().Be("Game");
            d.GetKeyType().Should().Be("string");
            d.GetComponentType().Should().Be("PrimaryEntityIndexComponent");
            d.GetMemberName().Should().Be("value");
        }

        [Fact]
        public void CreatesDataForMultiplePrimaryEntityIndexes()
        {
            var data = GetData<MultiplePrimaryEntityIndicesComponent, StandardComponent>();
            data.Length.Should().Be(2);
            data[0].GetEntityIndexName().Should().Be("MultiplePrimaryEntityIndices");
            data[0].GetHasMultiple().Should().BeTrue();
            data[1].GetEntityIndexName().Should().Be("MultiplePrimaryEntityIndices");
            data[1].GetHasMultiple().Should().BeTrue();
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
            d.GetEntityIndexType().Should().Be("MyNamespace.CustomEntityIndex");
            d.IsCustom().Should().BeTrue();
            d.GetEntityIndexName().Should().Be("MyNamespaceCustomEntityIndex");
            d.GetContextNames().Length.Should().Be(1);
            d.GetContextNames()[0].Should().Be("Test");

            var methods = d.GetCustomMethods();
            methods.GetType().Should().Be(typeof(MethodData[]));
            methods.Length.Should().Be(2);
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
            d.GetEntityIndexName().Should().Be("EntityIndex");
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
            d.GetContextNames().Length.Should().Be(1);
            d.GetContextNames()[0].Should().Be("ConfiguredContext");
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
