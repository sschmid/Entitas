using FluentAssertions;
using MyApp;
using MyFeature;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class ComponentTests
    {
        readonly MainContext _context;

        public ComponentTests()
        {
            ContextInitialization.Initialize();
            _context = new MainContext();
        }

        [Fact]
        public void DoesNotHaveComponent()
        {
            var entity = _context.CreateEntity();
            entity.HasPosition().Should().BeFalse();
        }

        [Fact]
        public void AddComponent()
        {
            var entity = _context.CreateEntity();
            entity.AddPosition(1, 2);
            entity.HasComponent(MyAppMainPositionComponentIndex.Index.Value);
        }

        [Fact]
        public void HasComponent()
        {
            var entity = _context.CreateEntity();
            entity.AddPosition(1, 2);
            entity.HasPosition().Should().BeTrue();
        }

        [Fact]
        public void GetComponent()
        {
            var entity = _context.CreateEntity();
            entity.AddPosition(1, 2);

            var position = entity.GetPosition();
            position.X.Should().Be(1);
            position.Y.Should().Be(2);
        }

        [Fact]
        public void ReplaceComponent()
        {
            var entity = _context.CreateEntity();
            entity.AddPosition(1, 2);
            entity.ReplacePosition(3, 4);

            var position = entity.GetPosition();
            position.X.Should().Be(3);
            position.Y.Should().Be(4);
        }

        [Fact]
        public void AddComponentUsesComponentPool()
        {
            var component = new PositionComponent { X = 1, Y = 2 };
            var entity = _context.CreateEntity();
            entity
                .GetComponentPool(MyAppMainPositionComponentIndex.Index.Value)
                .Push(component);

            entity.AddPosition(3, 4);
            entity.GetPosition().Should().BeSameAs(component);
        }

        [Fact]
        public void ReplaceComponentUsesComponentPool()
        {
            var component = new PositionComponent { X = 1, Y = 2 };
            var entity = _context.CreateEntity();
            entity.AddPosition(3, 4);
            entity
                .GetComponentPool(MyAppMainPositionComponentIndex.Index.Value)
                .Push(component);

            entity.ReplacePosition(5, 6);
            entity.GetPosition().Should().BeSameAs(component);
        }

        [Fact]
        public void UsesSingleComponent()
        {
            var entity1 = _context
                .CreateEntity()
                .AddMovable();

            var entity2 = _context
                .CreateEntity()
                .AddMovable();

            entity1.GetMovable().Should().BeSameAs(entity2.GetMovable());
        }
    }
}
