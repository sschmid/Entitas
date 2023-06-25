using FluentAssertions;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class ComponentIndexTests
    {
        [Fact]
        public void GeneratesComponentIndex()
        {
            new MyApp.Main.ComponentIndex(1).Value.Should().Be(1);
        }

        [Fact]
        public void EqualsComponentIndexWithSameIndex()
        {
            new MyApp.Main.ComponentIndex(1).Equals(new MyApp.Main.ComponentIndex(1)).Should().BeTrue();
        }

        [Fact]
        public void DoesNotEqualComponentIndexWithDifferentIndex()
        {
            new MyApp.Main.ComponentIndex(1).Equals(new MyApp.Main.ComponentIndex(2)).Should().BeFalse();
        }
    }
}
