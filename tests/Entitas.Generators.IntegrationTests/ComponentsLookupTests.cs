using FluentAssertions;
using MyApp.Main;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class ComponentsLookupTests
    {
        [Fact]
        public void AssignsComponentIndexes()
        {
            MyFeatureMovableComponentIndex.Index.Value.Should().Be(0);
            MyFeaturePositionComponentIndex.Index.Value.Should().Be(0);

            ComponentsLookup.AssignComponentIndexes();

            MyFeatureMovableComponentIndex.Index.Value.Should().Be(0);
            MyFeaturePositionComponentIndex.Index.Value.Should().Be(1);
        }

        [Fact]
        public void GeneratesNames()
        {
            ComponentsLookup.ComponentNames.Should().HaveCount(2);
        }

        [Fact]
        public void GeneratesTypes()
        {
            ComponentsLookup.ComponentTypes.Should().HaveCount(2);
        }
    }
}
