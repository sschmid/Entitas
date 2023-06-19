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
        public void ImplicitCastToInt()
        {
            int index = new MyApp.Main.ComponentIndex(1);
            index.Should().Be(1);
        }

        [Fact]
        public void ImplicitCastFromInt()
        {
            MyApp.Main.ComponentIndex componentIndex = 1;
            componentIndex.Value.Should().Be(1);
        }

        [Fact]
        public void EqualsComponentIndexWithSameIndex()
        {
            int index1 = new MyApp.Main.ComponentIndex(1);
            int index2 = new MyApp.Main.ComponentIndex(1);
            index1.Equals(index2).Should().BeTrue();
        }

        [Fact]
        public void DoesNotEqualComponentIndexWithDifferentIndex()
        {
            int index1 = new MyApp.Main.ComponentIndex(1);
            int index2 = new MyApp.Main.ComponentIndex(2);
            index1.Equals(index2).Should().BeFalse();
        }
    }
}
