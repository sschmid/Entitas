using FluentAssertions;
using MyApp;
using MyFeature;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class CleanupSystemTests
    {
        readonly MainContext _context;

        public CleanupSystemTests()
        {
            ContextInitialization.InitializeMain();
            _context = new MainContext();
        }

        [Fact]
        public void RemovesComponent()
        {
            var system = new RemoveMyAppMainUserCleanupSystem(_context);
            var entity = _context.CreateEntity().AddUser("Test", 42);
            system.Cleanup();
            entity.HasUser().Should().BeFalse();
            entity.isEnabled.Should().BeTrue();
        }

        [Fact]
        public void DestroysEntity()
        {
            var system = new DestroyMyAppMainLoadingCleanupSystem(_context);
            var entity = _context.CreateEntity().AddLoading();
            system.Cleanup();
            entity.HasLoading().Should().BeFalse();
            entity.isEnabled.Should().BeFalse();
        }
    }
}
