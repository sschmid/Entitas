using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class CollectorTests
    {
        readonly IContext<TestEntity> _context;
        readonly IGroup<TestEntity> _groupA;
        readonly IGroup<TestEntity> _groupB;

        public CollectorTests()
        {
            _context = new MyTestContext();
            _groupA = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
            _groupB = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentB));
        }

        [Fact]
        public void IsEmpty()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            collector.collectedEntities.Should().BeEmpty();
        }

        [Fact]
        public void ReturnsCollectedEntitiesOnAdded()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            var e = CreateEntityA();
            var entities = collector.collectedEntities;
            entities.Should().HaveCount(1);
            entities.Should().Contain(e);
        }

        [Fact]
        public void OnlyCollectsMatchingEntities()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            var e = CreateEntityA();
            CreateEntityB();
            var entities = collector.collectedEntities;
            entities.Should().HaveCount(1);
            entities.Should().Contain(e);
        }

        [Fact]
        public void CollectsEntitiesOnlyOnce()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            var e = CreateEntityA();
            e.RemoveComponentA();
            e.AddComponentA();
            var entities = collector.collectedEntities;
            entities.Should().HaveCount(1);
            entities.Should().Contain(e);
        }

        [Fact]
        public void ClearsCollectedEntities()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            CreateEntityA();
            collector.ClearCollectedEntities();
            collector.collectedEntities.Should().BeEmpty();
        }

        [Fact]
        public void ClearsCollectedEntitiesWhenDeactivating()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            CreateEntityA();
            collector.Deactivate();
            collector.collectedEntities.Should().BeEmpty();
        }

        [Fact]
        public void DoesNotCollectEntitiesWhenDeactivated()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            CreateEntityA();
            collector.Deactivate();
            CreateEntityA();
            collector.collectedEntities.Should().BeEmpty();
        }

        [Fact]
        public void ContinuesCollectingWhenActivated()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            CreateEntityA();
            collector.Deactivate();
            CreateEntityA();
            collector.Activate();
            var e = CreateEntityA();
            var entities = collector.collectedEntities;
            entities.Should().HaveCount(1);
            entities.Should().Contain(e);
        }

        [Fact]
        public void CanToString()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            CreateEntityA();
            collector.ToString().Should().Be("Collector(Group(AllOf(1)))");
        }

        [Fact]
        public void RetainsEntityEvenAfterDestroy()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            var e = CreateEntityA();
            e.Destroy();
            e.retainCount.Should().Be(1);
            (e.aerc as SafeAERC)?.owners.Should().Contain(collector);
        }

        [Fact]
        public void ReleasesEntityWhenClearingCollectedEntities()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            var e = CreateEntityA();
            e.Destroy();
            collector.ClearCollectedEntities();
            e.retainCount.Should().Be(0);
        }

        [Fact]
        public void RetainsEntitiesOnlyOnce()
        {
            var unused = new Collector<TestEntity>(_groupA, GroupEvent.Added);
            var e = CreateEntityA();
            e.ReplaceComponentA(new ComponentA());
            e.Destroy();
            e.retainCount.Should().Be(1);
        }

        [Fact]
        public void ReturnsCollectedEntitiesOnRemoved()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.Removed);
            var e = CreateEntityA();
            collector.collectedEntities.Should().BeEmpty();
            e.RemoveComponentA();
            var entities = collector.collectedEntities;
            entities.Should().HaveCount(1);
            entities.Should().Contain(e);
        }

        [Fact]
        public void ReturnsCollectedEntitiesOnAddedOrRemoved()
        {
            var collector = new Collector<TestEntity>(_groupA, GroupEvent.AddedOrRemoved);
            var e = CreateEntityA();
            var entities = collector.collectedEntities;
            entities.Should().HaveCount(1);
            entities.Should().Contain(e);
            collector.ClearCollectedEntities();
            e.RemoveComponentA();
            entities = collector.collectedEntities;
            entities.Should().HaveCount(1);
            entities.Should().Contain(e);
        }

        [Fact]
        public void ThrowsWhenGroupCountIsNotEqualGroupEventCount()
        {
            FluentActions.Invoking(() => new Collector<TestEntity>(
                new[] {_groupA},
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
                new[] {_groupA, _groupB},
                new[]
                {
                    GroupEvent.Added,
                    GroupEvent.Added
                }
            );

            var eA = CreateEntityA();
            var eB = CreateEntityB();
            var entities = collector.collectedEntities;
            entities.Should().HaveCount(2);
            entities.Should().Contain(eA);
            entities.Should().Contain(eB);
        }

        [Fact]
        public void CanToStringWithMultipleGroups()
        {
            var collector = new Collector<TestEntity>(
                new[] {_groupA, _groupB},
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
                new[] {_groupA, _groupB},
                new[]
                {
                    GroupEvent.Removed,
                    GroupEvent.Removed
                }
            );

            var eA = CreateEntityA();
            var eB = CreateEntityB();
            collector.collectedEntities.Should().BeEmpty();
            eA.RemoveComponentA();
            eB.RemoveComponentB();
            var entities = collector.collectedEntities;
            entities.Should().HaveCount(2);
            entities.Should().Contain(eA);
            entities.Should().Contain(eB);
        }

        [Fact]
        public void ReturnsCollectedEntitiesOnMultipleGroupsAddedOrRemoved()
        {
            var collector = new Collector<TestEntity>(
                new[] {_groupA, _groupB},
                new[]
                {
                    GroupEvent.AddedOrRemoved,
                    GroupEvent.AddedOrRemoved
                }
            );

            var eA = CreateEntityA();
            var eB = CreateEntityB();
            var entities = collector.collectedEntities;
            entities.Should().HaveCount(2);
            entities.Should().Contain(eA);
            entities.Should().Contain(eB);
            collector.ClearCollectedEntities();

            eA.RemoveComponentA();
            eB.RemoveComponentB();
            entities = collector.collectedEntities;
            entities.Should().HaveCount(2);
            entities.Should().Contain(eA);
            entities.Should().Contain(eB);
        }

        [Fact]
        public void ReturnsCollectedEntitiesOnMixedGroupEvents()
        {
            var collector = new Collector<TestEntity>(
                new[] {_groupA, _groupB},
                new[]
                {
                    GroupEvent.Added,
                    GroupEvent.Removed
                }
            );

            var eA = CreateEntityA();
            var eB = CreateEntityB();
            var entities = collector.collectedEntities;
            entities.Should().HaveCount(1);
            entities.Should().Contain(eA);
            collector.ClearCollectedEntities();

            eA.RemoveComponentA();
            eB.RemoveComponentB();
            entities = collector.collectedEntities;
            entities.Should().HaveCount(1);
            entities.Should().Contain(eB);
        }

        TestEntity CreateEntityA() => _context.CreateEntity().AddComponentA();
        TestEntity CreateEntityB() => _context.CreateEntity().AddComponentB();
    }
}
