using FluentAssertions;
using MyApp;
using MyApp.Main;
using MyFeature;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class ComponentTests
    {
        [Fact]
        public void DoesNotHaveComponent()
        {
            var entity = CreateContext().CreateEntity();
            entity.HasPosition().Should().BeFalse();
        }

        [Fact]
        public void AddComponent()
        {
            var entity = CreateContext().CreateEntity();
            entity.AddPosition(1, 2);
            entity.HasComponent(MyFeaturePositionComponentIndex.Index.Value);
        }

        [Fact]
        public void HasComponent()
        {
            var entity = CreateContext().CreateEntity();
            entity.AddPosition(1, 2);
            entity.HasPosition().Should().BeTrue();
        }

        [Fact]
        public void GetComponent()
        {
            var entity = CreateContext().CreateEntity();
            entity.AddPosition(1, 2);

            var position = entity.GetPosition();
            position.X.Should().Be(1);
            position.Y.Should().Be(2);
        }

        [Fact]
        public void DeconstructComponent()
        {
            var (x, y) = new PositionComponent { X = 1, Y = 2 };
            x.Should().Be(1);
            y.Should().Be(2);
        }

        [Fact]
        public void AddComponentUsesComponentPool()
        {
            var component = new PositionComponent { X = 1, Y = 2 };
            var entity = CreateContext().CreateEntity();
            entity
                .GetComponentPool(MyFeaturePositionComponentIndex.Index.Value)
                .Push(component);

            entity.AddPosition(3, 4);
            entity.GetPosition().Should().BeSameAs(component);
        }

        [Fact]
        public void ReplaceComponentUsesComponentPool()
        {
            var component = new PositionComponent { X = 1, Y = 2 };
            var entity = CreateContext().CreateEntity();
            entity.AddPosition(3, 4);
            entity
                .GetComponentPool(MyFeaturePositionComponentIndex.Index.Value)
                .Push(component);

            entity.ReplacePosition(5, 6);
            entity.GetPosition().Should().BeSameAs(component);
        }

        [Fact]
        public void UsesSingleComponent()
        {
            var entity1 = CreateContext()
                .CreateEntity()
                .AddMovable();

            var entity2 = CreateContext()
                .CreateEntity()
                .AddMovable();

            entity1.GetMovable().Should().BeSameAs(entity2.GetMovable());
        }

        static IContext<MyApp.Main.Entity> CreateContext()
        {
            ComponentsLookup.AssignComponentIndexes();
            return new MainContext();
        }
    }
}
