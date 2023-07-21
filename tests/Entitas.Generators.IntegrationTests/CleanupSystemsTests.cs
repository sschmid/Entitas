using FluentAssertions;
using MyApp;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class CleanupSystemsTests
    {
        [Fact]
        public void GeneratesCleanupSystems()
        {
            ContextInitialization.InitializeMain();
            new MainContext().CreateCleanupSystems().Should().NotBeNull();
        }

        [Fact]
        public void GeneratesEmptyCleanupSystems()
        {
            ContextInitialization.InitializeEmpty();
            new EmptyContext().CreateCleanupSystems().Should().BeNull();
        }
    }
}
