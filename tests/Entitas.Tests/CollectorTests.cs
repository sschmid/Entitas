using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class CollectorTests
    {
        readonly TestContext _context;
        readonly IGroup<TestEntity> _groupA;
        readonly IGroup<TestEntity> _groupB;

        public CollectorTests()
        {
            _context = new TestContext(CID.TotalComponents);
            _groupA = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
            _groupB = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentB));
        }

        [Fact]
        public void IsEmpty()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            collector.CollectedEntities.Should().BeEmpty();
        }

        [Fact]
        public void ReturnsCollectedEntitiesOnAdded()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            var entity = _context.CreateEntity().AddComponentA();
            var entities = collector.CollectedEntities;
            entities.Should().HaveCount(1);
            entities.Should().Contain(entity);
        }

        [Fact]
        public void OnlyCollectsMatchingEntities()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            var entity = _context.CreateEntity().AddComponentA();
            var unused = _context.CreateEntity().AddComponentB();
            var entities = collector.CollectedEntities;
            entities.Should().HaveCount(1);
            entities.Should().Contain(entity);
        }

        [Fact]
        public void CollectsEntitiesOnlyOnce()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            var entity = _context.CreateEntity()
                .AddComponentA()
                .RemoveComponentA()
                .AddComponentA();
            var entities = collector.CollectedEntities;
            entities.Should().HaveCount(1);
            entities.Should().Contain(entity);
        }

        [Fact]
        public void ClearsCollectedEntities()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            _context.CreateEntity().AddComponentA();
            collector.ClearCollectedEntities();
            collector.CollectedEntities.Should().BeEmpty();
        }

        [Fact]
        public void ClearsCollectedEntitiesWhenDeactivating()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            _context.CreateEntity().AddComponentA();
            collector.Deactivate();
            collector.CollectedEntities.Should().BeEmpty();
        }

        [Fact]
        public void DoesNotCollectEntitiesWhenDeactivated()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            _context.CreateEntity().AddComponentA();
            collector.Deactivate();
            _context.CreateEntity().AddComponentA();
            collector.CollectedEntities.Should().BeEmpty();
        }

        [Fact]
        public void ContinuesCollectingWhenActivated()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            _context.CreateEntity().AddComponentA();
            collector.Deactivate();
            _context.CreateEntity().AddComponentA();
            collector.Activate();
            var entity = _context.CreateEntity().AddComponentA();
            var entities = collector.CollectedEntities;
            entities.Should().HaveCount(1);
            entities.Should().Contain(entity);
        }

        [Fact]
        public void CanToString()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            _context.CreateEntity().AddComponentA();
            collector.ToString().Should().Be("Collector(Group(AllOf(1)))");
        }

        [Fact]
        public void RetainsEntityEvenAfterDestroy()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            var entity = _context.CreateEntity().AddComponentA();
            entity.Destroy();
            entity.RetainCount.Should().Be(1);
            (entity.Aerc as SafeAERC)!.Owners.Should().Contain(collector);
        }

        [Fact]
        public void ReleasesEntityWhenClearingCollectedEntities()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            var entity = _context.CreateEntity().AddComponentA();
            entity.Destroy();
            collector.ClearCollectedEntities();
            entity.RetainCount.Should().Be(0);
        }

        [Fact]
        public void RetainsEntitiesOnlyOnce()
        {
            var unused = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            var entity = _context.CreateEntity()
                .AddComponentA()
                .ReplaceComponentA(new ComponentA());
            entity.Destroy();
            entity.RetainCount.Should().Be(1);
        }

        [Fact]
        public void ReturnsCollectedEntitiesOnRemoved()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Removed);
            var entity = _context.CreateEntity().AddComponentA();
            collector.CollectedEntities.Should().BeEmpty();
            entity.RemoveComponentA();
            var entities = collector.CollectedEntities;
            entities.Should().HaveCount(1);
            entities.Should().Contain(entity);
        }

        [Fact]
        public void ReturnsCollectedEntitiesOnAddedOrRemoved()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.AddedOrRemoved);
            var entity = _context.CreateEntity().AddComponentA();
            var entities = collector.CollectedEntities;
            entities.Should().HaveCount(1);
            entities.Should().Contain(entity);
            collector.ClearCollectedEntities();
            entity.RemoveComponentA();
            entities = collector.CollectedEntities;
            entities.Should().HaveCount(1);
            entities.Should().Contain(entity);
        }

        [Fact]
        public void ThrowsWhenGroupCountIsNotEqualGroupEventCount()
        {
            FluentActions.Invoking(() => new Collector<TestEntity>(
                new[] { _groupA },
                new[]
                {
                    GroupEvent.Added,
                    GroupEvent.Added
                }
            )).Should().Throw<CollectorException>();
        }

        [Fact]
        public void ReturnsCollectedEntitiesOnMultipleGroupsAdded()
        {
            var collector = new Collector<TestEntity>(
                new[] { _groupA, _groupB },
                new[]
                {
                    GroupEvent.Added,
                    GroupEvent.Added
                }
            );

            var entityA = _context.CreateEntity().AddComponentA();
            var entityB = _context.CreateEntity().AddComponentB();
            var entities = collector.CollectedEntities;
            entities.Should().HaveCount(2);
            entities.Should().Contain(entityA);
            entities.Should().Contain(entityB);
        }

        [Fact]
        public void CanToStringWithMultipleGroups()
        {
            var collector = new Collector<TestEntity>(
                new[] { _groupA, _groupB },
                new[]
                {
                    GroupEvent.Added,
                    GroupEvent.Added
                }
            );

            collector.ToString().Should().Be("Collector(Group(AllOf(1)), Group(AllOf(2)))");
        }

        [Fact]
        public void ReturnsCollectedEntitiesOnMultipleGroupsRemoved()
        {
            var collector = new Collector<TestEntity>(
                new[] { _groupA, _groupB },
                new[]
                {
                    GroupEvent.Removed,
                    GroupEvent.Removed
                }
            );

            var entityA = _context.CreateEntity().AddComponentA();
            var entityB = _context.CreateEntity().AddComponentB();
            collector.CollectedEntities.Should().BeEmpty();
            entityA.RemoveComponentA();
            entityB.RemoveComponentB();
            var entities = collector.CollectedEntities;
            entities.Should().HaveCount(2);
            entities.Should().Contain(entityA);
            entities.Should().Contain(entityB);
        }

        [Fact]
        public void ReturnsCollectedEntitiesOnMultipleGroupsAddedOrRemoved()
        {
            var collector = new Collector<TestEntity>(
                new[] { _groupA, _groupB },
                new[]
                {
                    GroupEvent.AddedOrRemoved,
                    GroupEvent.AddedOrRemoved
                }
            );

            var entityA = _context.CreateEntity().AddComponentA();
            var entityB = _context.CreateEntity().AddComponentB();
            var entities = collector.CollectedEntities;
            entities.Should().HaveCount(2);
            entities.Should().Contain(entityA);
            entities.Should().Contain(entityB);
            collector.ClearCollectedEntities();

            entityA.RemoveComponentA();
            entityB.RemoveComponentB();
            entities = collector.CollectedEntities;
            entities.Should().HaveCount(2);
            entities.Should().Contain(entityA);
            entities.Should().Contain(entityB);
        }

        [Fact]
        public void ReturnsCollectedEntitiesOnMixedGroupEvents()
        {
            var collector = new Collector<TestEntity>(
                new[] { _groupA, _groupB },
                new[]
                {
                    GroupEvent.Added,
                    GroupEvent.Removed
                }
            );

            var entityA = _context.CreateEntity().AddComponentA();
            var entityB = _context.CreateEntity().AddComponentB();
            var entities = collector.CollectedEntities;
            entities.Should().HaveCount(1);
            entities.Should().Contain(entityA);
            collector.ClearCollectedEntities();

            entityA.RemoveComponentA();
            entityB.RemoveComponentB();
            entities = collector.CollectedEntities;
            entities.Should().HaveCount(1);
            entities.Should().Contain(entityB);
        }
    }
}
