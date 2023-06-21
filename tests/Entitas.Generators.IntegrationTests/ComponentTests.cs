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

        [Fact]
        public void GeneratesComponentEntityExtension()
        {
            var entity = CreateContext().CreateEntity();

            entity.HasPosition().Should().BeFalse();

            entity.AddPosition(1, 2);

            entity.HasPosition().Should().BeTrue();
            var position = entity.GetPosition();
            position.X.Should().Be(1);
            position.Y.Should().Be(2);
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
