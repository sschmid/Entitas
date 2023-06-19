using FluentAssertions;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class ContextTests
    {
        [Fact]
        public void GeneratesContext()
        {
            var context = new MyApp.MainContext();
            context.Should().NotBeNull();
            context.Should().BeAssignableTo<Context<MyApp.Main.Entity>>();
        }
    }
}
