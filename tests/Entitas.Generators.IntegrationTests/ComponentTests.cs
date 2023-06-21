using System;
using System.Linq;
using FluentAssertions;
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
            entity.HasComponent(MyFeaturePositionComponentIndex.Value);
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
                .GetComponentPool(MyFeaturePositionComponentIndex.Value)
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
                .GetComponentPool(MyFeaturePositionComponentIndex.Value)
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

        static TestContext<MyApp.Main.Entity> CreateContext()
        {
            MyFeatureMovableComponentIndex.Value = 0;
            MyFeaturePositionComponentIndex.Value = 1;
            return new TestContext<MyApp.Main.Entity>(
                2,
                new[]
                {
                    typeof(MovableComponent),
                    typeof(PositionComponent)
                });
        }
    }

    public sealed class TestContext<TEntity> : Context<TEntity> where TEntity : class, IEntity, new()
    {
        public TestContext(int totalComponents, Type[] componentTypes)
            : base(
                totalComponents,
                0,
                new ContextInfo(
                    "Entitas.Generators.IntegrationTests.TestContext",
                    componentTypes.Select(type => type.FullName).ToArray(),
                    componentTypes
                ),
                entity => new SafeAERC(entity),
                () => new TEntity()
            ) { }
    }
}
