using FluentAssertions;
using MyApp;
using MyFeature;
using Xunit;
using static MyApp.MainContextEntityIndexExtension;

namespace Entitas.Generators.IntegrationTests
{
    public class EntityIndexExtensionTests
    {
        readonly MainContext _context;

        public EntityIndexExtensionTests()
        {
            ContextInitialization.InitializeMain();
            _context = new MainContext();
            _context.AddAllEntityIndexes();
        }

        [Fact]
        public void AddsAllEntityIndexes()
        {
            _context.GetEntityIndex(MyFeatureUserName).Should().BeAssignableTo<PrimaryEntityIndex<MyApp.Main.Entity, string>>();
            _context.GetEntityIndex(MyFeatureUserAge).Should().BeAssignableTo<EntityIndex<MyApp.Main.Entity, int>>();
        }

        [Fact]
        public void GetsEntity()
        {
            var user = _context.CreateEntity().AddUser("Test", 42);
            var entity = _context.GetEntityWithUserName("Test");

            entity.Should().BeSameAs(user);
        }

        [Fact]
        public void GetsEntities()
        {
            var user1 = _context.CreateEntity().AddUser("Test1", 42);
            var user2 = _context.CreateEntity().AddUser("Test2", 42);
            var entities = _context.GetEntitiesWithUserAge(42);

            entities.Should().HaveCount(2);
            entities.Should().Contain(user1);
            entities.Should().Contain(user2);
        }
    }
}
