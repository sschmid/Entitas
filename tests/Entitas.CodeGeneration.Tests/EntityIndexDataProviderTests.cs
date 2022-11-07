using DesperateDevs.Serialization;
using Entitas.CodeGeneration.Plugins;
using FluentAssertions;
using My.Namespace;
using MyNamespace;
using Xunit;

namespace Entitas.CodeGeneration.Tests
{
    public class EntityIndexDataProviderTests
    {
        [Fact]
        public void CreatesDataForSingleEntityIndex()
        {
            var data = GetData<EntityIndexComponent, StandardComponent>();
            data.Length.Should().Be(1);

            var d = data[0];

            d.GetEntityIndexType().GetType().Should().Be(typeof(string));
            d.GetEntityIndexType().Should().Be("Entitas.EntityIndex");

            d.IsCustom().GetType().Should().Be(typeof(bool));
            d.IsCustom().Should().BeFalse();

            d.GetEntityIndexName().GetType().Should().Be(typeof(string));
            d.GetEntityIndexName().Should().Be("MyNamespaceEntityIndex");

            d.GetContextNames().GetType().Should().Be(typeof(string[]));
            d.GetContextNames().Length.Should().Be(2);
            d.GetContextNames()[0].Should().Be("Test");
            d.GetContextNames()[1].Should().Be("Test2");

            d.GetKeyType().GetType().Should().Be(typeof(string));
            d.GetKeyType().Should().Be("string");

            d.GetComponentType().GetType().Should().Be(typeof(string));
            d.GetComponentType().Should().Be("My.Namespace.EntityIndexComponent");

            d.GetMemberName().GetType().Should().Be(typeof(string));
            d.GetMemberName().Should().Be("value");

            d.GetHasMultiple().GetType().Should().Be(typeof(bool));
            d.GetHasMultiple().Should().BeFalse();
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
            d.GetHasMultiple().Should().BeFalse();
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
            GetData<AbstractEntityIndexComponent, StandardComponent>().Should().HaveCount(0);
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
            GetData<ClassWithEntitIndexAttribute, StandardComponent>().Should().HaveCount(0);
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

        EntityIndexData[] GetData<T1, T2>(Preferences preferences = null)
        {
            var provider = new EntityIndexDataProvider(new[] {typeof(T1), typeof(T2)});
            preferences ??= new TestPreferences(
                "Entitas.CodeGeneration.Plugins.Contexts = Game, GameState" + "\n" +
                "Entitas.CodeGeneration.Plugins.IgnoreNamespaces = false"
            );

            provider.Configure(preferences);
            return (EntityIndexData[])provider.GetData();
        }
    }
}
