using FluentAssertions;
using MyApp;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class ContextTests
    {
        public ContextTests()
        {
            ContextInitialization.Initialize();
        }

        [Fact]
        public void GeneratesContext()
        {
            var context = new MyApp.MainContext();
            context.Should().NotBeNull();
            context.Should().BeAssignableTo<Context<MyApp.Main.Entity>>();
        }
    }
}
