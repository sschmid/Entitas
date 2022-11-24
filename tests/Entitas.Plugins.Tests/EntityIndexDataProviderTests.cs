using DesperateDevs.Serialization;
using FluentAssertions;
using My.Namespace;
using MyNamespace;
using Xunit;

namespace Entitas.Plugins.Tests
{
    public class EntityIndexDataProviderTests
    {
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
            d.MemberName.Should().Be("Value");
            d.HasMultiple.Should().BeFalse();
        }

        [Fact]
        public void CreatesDataForMultipleEntityIndexes()
        {
            var data = GetData<MultipleEntityIndexesComponent, StandardComponent>();
            data.Length.Should().Be(2);

            data[0].Name.Should().Be("MyNamespaceMultipleEntityIndexes");
            data[0].HasMultiple.Should().BeTrue();

            data[1].Name.Should().Be("MyNamespaceMultipleEntityIndexes");
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
            d.MemberName.Should().Be("Value");
            d.HasMultiple.Should().BeFalse();
        }

        [Fact]
        public void CreatesDataForMultiplePrimaryEntityIndexes()
        {
            var data = GetData<MultiplePrimaryEntityIndexesComponent, StandardComponent>();
            data.Length.Should().Be(2);

            data[0].Name.Should().Be("MultiplePrimaryEntityIndexes");
            data[0].HasMultiple.Should().BeTrue();

            data[1].Name.Should().Be("MultiplePrimaryEntityIndexes");
            data[1].HasMultiple.Should().BeTrue();
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

            d.Type.Should().Be("MyNamespace.CustomEntityIndex");
            d.IsCustom.Should().BeTrue();
            d.Name.Should().Be("MyNamespaceCustomEntityIndex");
            d.Contexts.Length.Should().Be(1);
            d.Contexts[0].Should().Be("Test1");

            var methods = d.CustomMethods;
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
                "Entitas.Plugins.Contexts = ConfiguredContext" + "\n" +
                "Entitas.Plugins.IgnoreNamespaces = true"
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
                "Entitas.Plugins.Contexts = ConfiguredContext" + "\n" +
                "Entitas.Plugins.IgnoreNamespaces = true"
            );

            var data = GetData<EntityIndexNoContextComponent, StandardComponent>(preferences);
            data.Length.Should().Be(1);
            var d = data[0];
            d.Contexts.Length.Should().Be(1);
            d.Contexts[0].Should().Be("ConfiguredContext");
        }

        EntityIndexData[] GetData<T1, T2>(Preferences preferences = null)
        {
            var provider = new EntityIndexDataProvider(new[] {typeof(T1), typeof(T2)});
            preferences ??= new TestPreferences(
                "Entitas.Plugins.Contexts = Game, GameState" + "\n" +
                "Entitas.Plugins.IgnoreNamespaces = false"
            );

            provider.Configure(preferences);
            return (EntityIndexData[])provider.GetData();
        }
    }
}
