using FluentAssertions;
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
    }
}
