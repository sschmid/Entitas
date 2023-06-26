using FluentAssertions;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class ContextTests
    {
        [Fact]
        public void GeneratesContext()
        {
            MyApp.Main.ComponentsLookup.AssignComponentIndexes();
            var context = new MyApp.MainContext();
            context.Should().NotBeNull();
            context.Should().BeAssignableTo<Context<MyApp.Main.Entity>>();
        }
    }
}
