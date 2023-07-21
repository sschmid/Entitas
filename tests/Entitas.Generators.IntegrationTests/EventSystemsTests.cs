using FluentAssertions;
using MyApp;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class EventSystemsTests
    {
        [Fact]
        public void GeneratesEventSystems()
        {
            ContextInitialization.InitializeMain();
            new MainContext().CreateEventSystems().Should().NotBeNull();
        }

        [Fact]
        public void GeneratesEmptyEventSystems()
        {
            ContextInitialization.InitializeEmpty();
            new EmptyContext().CreateEventSystems().Should().BeNull();
        }
    }
}
