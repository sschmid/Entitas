using FluentAssertions;
using MyFeature;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class ContextAttributeTests
    {
        [Fact]
        public void ContextAttribute()
        {
            new MyApp.Main.ContextAttribute().Should().BeAssignableTo<System.Attribute>();
        }

        [Fact]
        public void IsStrippedByCompiler()
        {
            typeof(MovableComponent).CustomAttributes.Should().HaveCount(0);
        }
    }
}
