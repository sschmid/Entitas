using FluentAssertions;
using MyApp;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class ContextTests
    {
        public ContextTests()
        {
            ContextInitialization.InitializeMain();
        }

        [Fact]
        public void GeneratesContext()
        {
            var context = new MainContext();
            context.Should().NotBeNull();
            context.Should().BeAssignableTo<Context<MyApp.Main.Entity>>();
        }

        [Fact]
        public void CreatesEntity()
        {
            var context = new MainContext();
            var entity = context.CreateEntity();
            entity.Should().NotBeNull();
            entity.Should().BeAssignableTo<MyApp.Main.Entity>();
            entity.TotalComponents.Should().Be(context.TotalComponents);
        }
    }
}
