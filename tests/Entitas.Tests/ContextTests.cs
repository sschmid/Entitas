using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class ContextTests
    {
        readonly IContext<TestEntity> _context;
        readonly ContextInfo _contextInfo;
        readonly IAllOfMatcher<TestEntity> _matcherAB = Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB);

        public ContextTests()
        {
            _contextInfo = new ContextInfo(
                "Test Context",
                new[] { "Health", "Position", "View" },
                new[] { typeof(ComponentA), typeof(ComponentB), typeof(ComponentC) });

            _context = new TestContext(CID.TotalComponents);
        }

        [Fact]
        public void IncrementsCreationIndex()
        {
            _context.CreateEntity().Id.Should().Be(0);
            _context.CreateEntity().Id.Should().Be(1);
        }

        [Fact]
        public void StartsWithGivenCreationIndex()
        {
            var context = new TestContext(_contextInfo.ComponentNames.Length, 42, _contextInfo);
            context.CreateEntity().Id.Should().Be(42);
        }

        [Fact]
        public void IsEmpty()
        {
            _context.GetEntities().Should().BeEmpty();
        }

        [Fact]
        public void TotalEntityCountIsZero()
        {
            _context.Count.Should().Be(0);
        }

        [Fact]
        public void CreatesEntity()
        {
            var entity = _context.CreateEntity();
            entity.Should().NotBeNull();
            entity.GetType().Should().Be(typeof(TestEntity));
            entity.TotalComponents.Should().Be(_context.TotalComponents);
            entity.IsEnabled.Should().BeTrue();
        }

        [Fact]
        public void HasDefaultContextInfo()
        {
            _context.ContextInfo.Name.Should().Be("Unnamed Context");
            _context.ContextInfo.ComponentNames.Length.Should().Be(CID.TotalComponents);
            for (var i = 0; i < _context.ContextInfo.ComponentNames.Length; i++)
                _context.ContextInfo.ComponentNames[i].Should().Be($"Index {i}");
        }

        [Fact]
        public void CreatesComponentPools()
        {
            _context.ComponentPools.Should().NotBeNull();
            _context.ComponentPools.Length.Should().Be(CID.TotalComponents);
        }

        [Fact]
        public void CreatesEntityWithComponentPools()
        {
            _context.CreateEntity().ComponentPools
                .Should().BeSameAs(_context.ComponentPools);
        }

        [Fact]
        public void CanToString()
        {
            _context.ToString().Should().Be("Unnamed Context");
        }

        [Fact]
        public void HasCustomContextInfo()
        {
            var context = new TestContext(_contextInfo.ComponentNames.Length, 0, _contextInfo);
            context.ContextInfo.Should().BeSameAs(_contextInfo);
        }

        [Fact]
        public void CreatesEntityWithSameContextInfo()
        {
            var context = new TestContext(_contextInfo.ComponentNames.Length, 0, _contextInfo);
            context.CreateEntity().ContextInfo.Should().BeSameAs(_contextInfo);
        }

        [Fact]
        public void ThrowsWhenComponentNamesLengthIsNotEqualToTotalComponents()
        {
            FluentActions.Invoking(() => new TestContext(_contextInfo.ComponentNames.Length + 1, 0, _contextInfo))
                .Should().Throw<ContextInfoException>();
        }

        [Fact]
        public void GetsTotalEntityCount()
        {
            _context.CreateEntity().AddComponentA();
            _context.Count.Should().Be(1);
        }

        [Fact]
        public void HasEntitiesThatWereCreatedWithCreateEntity()
        {
            var entity = _context.CreateEntity().AddComponentA();
            _context.HasEntity(entity).Should().BeTrue();
        }

        [Fact]
        public void ReturnsAllCreatedEntities()
        {
            var entity1 = _context.CreateEntity();
            var entity2 = _context.CreateEntity();
            var entities = _context.GetEntities();
            entities.Length.Should().Be(2);
            entities.Should().Contain(entity1);
            entities.Should().Contain(entity2);
        }

        [Fact]
        public void DestroysEntityAndRemovesIt()
        {
            var entity = _context.CreateEntity();
            entity.Destroy();
            _context.HasEntity(entity).Should().BeFalse();
            _context.Count.Should().Be(0);
            _context.GetEntities().Should().BeEmpty();
        }

        [Fact]
        public void DestroysEntityAndRemovesAllComponents()
        {
            var entity = _context.CreateEntity();
            entity.Destroy();
            entity.GetComponents().Should().BeEmpty();
        }

        [Fact]
        public void RemovesOnDestroyEntityHandler()
        {
            var entity = _context.CreateEntity();
            var didDestroy = 0;
            _context.OnEntityWillBeDestroyed += delegate { didDestroy += 1; };
            entity.Destroy();
            _context.CreateEntity().Destroy();
            didDestroy.Should().Be(2);
        }

        [Fact]
        public void DestroysAllEntities()
        {
            var testEntity = _context.CreateEntity();
            _context.CreateEntity();
            _context.DestroyAllEntities();
            _context.HasEntity(testEntity).Should().BeFalse();
            _context.Count.Should().Be(0);
            _context.GetEntities().Should().BeEmpty();
            testEntity.GetComponents().Should().BeEmpty();
        }

        [Fact]
        public void EnsuresSameDeterministicOrderWhenGettingEntitiesAfterDestroyingAllEntities()
        {
            // This is a Unity specific problem. Run Unity Test Tools in the Unity project

            const int numEntities = 10;

            for (var i = 0; i < numEntities; i++)
                _context.CreateEntity();

            var order1 = _context.GetEntities().Select(e => e.Id).ToArray();

            _context.Reset();

            for (var i = 0; i < numEntities; i++)
                _context.CreateEntity();

            var order2 = _context.GetEntities().Select(e => e.Id).ToArray();

            order1.Should().BeEquivalentTo(order2);
        }

        [Fact]
        public void ThrowsWhenDestroyingAllEntitiesWithRetainedEntities()
        {
            FluentActions.Invoking(() =>
                {
                    _context.CreateEntity().Retain(new object());
                    _context.DestroyAllEntities();
                }
            ).Should().Throw<ContextStillHasRetainedEntitiesException>();
        }

        [Fact]
        public void CachesEntities()
        {
            _context.GetEntities().Should().BeSameAs(_context.GetEntities());
        }

        [Fact]
        public void UpdatesEntitiesCacheWhenCreatingEntity()
        {
            var entities = _context.GetEntities();
            _context.CreateEntity();
            _context.GetEntities().Should().NotBeSameAs(entities);
        }

        [Fact]
        public void UpdatesEntitiesCacheWhenDestroyingEntity()
        {
            var entity = _context.CreateEntity();
            var entities = _context.GetEntities();
            entity.Destroy();
            _context.GetEntities().Should().NotBeSameAs(entities);
        }

        [Fact]
        public void DispatchesOnEntityCreatedWhenCreatingNewEntity()
        {
            var didDispatch = 0;
            IEntity eventEntity = null;
            _context.OnEntityCreated += (c, e) =>
            {
                didDispatch += 1;
                eventEntity = e;
                c.Should().BeSameAs(_context);
            };

            var entity = _context.CreateEntity();
            didDispatch.Should().Be(1);
            eventEntity.Should().BeSameAs(entity);
        }

        [Fact]
        public void DispatchesOnEntityWillBeDestroyedWhenDestroyingEntity()
        {
            var entity = _context.CreateEntity().AddComponentA();
            var didDispatch = 0;
            _context.OnEntityWillBeDestroyed += (c, e) =>
            {
                didDispatch += 1;
                c.Should().BeSameAs(_context);
                e.Should().BeSameAs(entity);
                e.HasComponentA().Should().BeTrue();
                e.IsEnabled.Should().BeTrue();
                c.Count.Should().Be(0);
            };
            entity.Destroy();
            didDispatch.Should().Be(1);
        }

        [Fact]
        public void DispatchesOnEntityDestroyedWhenDestroyingEntity()
        {
            var entity = _context.CreateEntity();
            var didDispatch = 0;
            _context.OnEntityDestroyed += (p, e) =>
            {
                didDispatch += 1;
                p.Should().BeSameAs(_context);
                e.Should().BeSameAs(entity);
                e.HasComponentA().Should().BeFalse();
                e.IsEnabled.Should().BeFalse();
            };
            entity.Destroy();
            didDispatch.Should().Be(1);
        }

        [Fact]
        public void ReleasesEntityAfterOnEntityDestroyed()
        {
            var entity = _context.CreateEntity();
            var didDispatch = 0;
            _context.OnEntityDestroyed += (_, e) =>
            {
                didDispatch += 1;
                e.RetainCount.Should().Be(1);
                var newEntity = _context.CreateEntity();
                newEntity.Should().NotBeNull();
                newEntity.Should().NotBeSameAs(e);
            };
            entity.Destroy();
            var reusedEntity = _context.CreateEntity();
            reusedEntity.Should().BeSameAs(entity);
            didDispatch.Should().Be(1);
        }

        [Fact]
        public void ThrowsIfEntityIsReleasedBeforeItIsDestroyed()
        {
            FluentActions.Invoking(() => _context.CreateEntity().Release(_context))
                .Should().Throw<EntityIsNotDestroyedException>();
        }

        [Fact]
        public void DispatchesOnGroupCreatedWhenCreatingNewGroup()
        {
            var didDispatch = 0;
            IGroup eventGroup = null;
            _context.OnGroupCreated += (p, g) =>
            {
                didDispatch += 1;
                p.Should().BeSameAs(_context);
                eventGroup = g;
            };
            var group = _context.GetGroup(Matcher<TestEntity>.AllOf(0));
            didDispatch.Should().Be(1);
            eventGroup.Should().BeSameAs(group);
        }

        [Fact]
        public void DoesNotDispatchOnGroupCreatedWhenGroupAlreadyExists()
        {
            _context.GetGroup(Matcher<TestEntity>.AllOf(0));
            _context.OnGroupCreated += delegate { throw new Exception("context.OnGroupCreated"); };
            _context.GetGroup(Matcher<TestEntity>.AllOf(0));
        }

        [Fact]
        public void RemovesEventHandlersWhenDestroyingEntity()
        {
            var entity1 = _context.CreateEntity();
            entity1.OnComponentAdded += delegate { throw new Exception("entity.OnComponentAdded"); };
            entity1.OnComponentRemoved += delegate { throw new Exception("entity.OnComponentRemoved"); };
            entity1.OnComponentReplaced += delegate { throw new Exception("entity.OnComponentReplaced"); };
            entity1.Destroy();

            var entity2 = _context.CreateEntity();
            entity2.Should().BeSameAs(entity1);
            entity2.AddComponentA()
                .ReplaceComponentA(Component.A)
                .RemoveComponentA();
        }

        [Fact]
        public void WillNotRemoveOnEntityReleased()
        {
            var entity = _context.CreateEntity();
            var didRelease = 0;
            entity.OnEntityReleased += delegate { didRelease += 1; };
            entity.Destroy();
            didRelease.Should().Be(1);
        }

        [Fact]
        public void RemovesOnEntityReleasedAfterBeingDispatched()
        {
            var entity = _context.CreateEntity();
            var didRelease = 0;
            entity.OnEntityReleased += delegate { didRelease += 1; };
            entity.Destroy();
            entity.Retain(this);
            entity.Release(this);
            didRelease.Should().Be(1);
        }

        [Fact]
        public void RemovesOnEntityReleasedAfterBeingDispatchedWhenDelayedRelease()
        {
            var entity = _context.CreateEntity();
            var didRelease = 0;
            entity.OnEntityReleased += delegate { didRelease += 1; };
            entity.Retain(this);
            entity.Destroy();
            didRelease.Should().Be(0);

            entity.Release(this);
            didRelease.Should().Be(1);

            entity.Retain(this);
            entity.Release(this);
            didRelease.Should().Be(1);
        }

        [Fact]
        public void ReturnsPushedEntity()
        {
            var entity1 = _context.CreateEntity().AddComponentA();
            entity1.Destroy();
            var entity2 = _context.CreateEntity();
            entity2.HasComponent(CID.ComponentA).Should().BeFalse();
            entity2.Should().BeSameAs(entity1);
        }

        [Fact]
        public void OnlyReturnsReleasedEntities()
        {
            var entity1 = _context.CreateEntity();
            entity1.Retain(this);
            entity1.Destroy();

            var entity2 = _context.CreateEntity();
            entity2.Should().NotBeSameAs(entity1);
            entity1.Release(this);

            var entity3 = _context.CreateEntity();
            entity3.Should().BeSameAs(entity1);
        }

        [Fact]
        public void ReturnsNewEntity()
        {
            var entity1 = _context.CreateEntity().AddComponentA();
            entity1.Destroy();
            _context.CreateEntity();

            var entity2 = _context.CreateEntity();
            entity2.HasComponent(CID.ComponentA).Should().BeFalse();
            entity2.Should().NotBeSameAs(entity1);
        }

        [Fact]
        public void SetsUpEntityFromObjectPool()
        {
            var entity = _context.CreateEntity();
            var id = entity.Id;
            entity.Destroy();
            var group = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));

            entity = _context.CreateEntity();
            entity.Id.Should().Be(id + 1);
            entity.IsEnabled.Should().BeTrue();

            entity.AddComponentA();
            group.GetEntities().Should().Contain(entity);
        }

        [Fact]
        public void ThrowsWhenAddingComponentToDestroyedEntity()
        {
            var entity = _context.CreateEntity().AddComponentA();
            entity.Destroy();
            FluentActions.Invoking(() => entity.AddComponentA())
                .Should().Throw<EntityIsNotEnabledException>();
        }

        [Fact]
        public void ThrowsWhenRemovingComponentFromDestroyedEntity()
        {
            var entity = _context.CreateEntity().AddComponentA();
            entity.Destroy();
            FluentActions.Invoking(() => entity.RemoveComponentA())
                .Should().Throw<EntityIsNotEnabledException>();
        }

        [Fact]
        public void ThrowsWhenReplacingComponentOnDestroyedEntity()
        {
            var entity = _context.CreateEntity().AddComponentA();
            entity.Destroy();
            FluentActions.Invoking(() => entity.ReplaceComponentA(new ComponentA()))
                .Should().Throw<EntityIsNotEnabledException>();
        }

        [Fact]
        public void ThrowsWhenReplacingComponentWithNullOnDestroyedEntity()
        {
            var entity = _context.CreateEntity().AddComponentA();
            entity.Destroy();
            FluentActions.Invoking(() => entity.ReplaceComponentA(null))
                .Should().Throw<EntityIsNotEnabledException>();
        }

        [Fact]
        public void ThrowsWhenDestroyingDestroyedEntity()
        {
            var entity = _context.CreateEntity().AddComponentA();
            entity.Destroy();
            FluentActions.Invoking(() => entity.Destroy())
                .Should().Throw<EntityIsNotEnabledException>();
        }

        [Fact]
        public void GetsEmptyGroupForMatcherWhenNoEntitiesWereCreated()
        {
            var group = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
            group.Should().NotBeNull();
            group.GetEntities().Should().BeEmpty();
        }

        [Fact]
        public void GetsGroupWithMatchingEntities()
        {
            var entity1 = _context.CreateEntity();
            entity1.AddComponentA();
            entity1.AddComponentB();

            var entity2 = _context.CreateEntity();
            entity2.AddComponentA();
            entity2.AddComponentB();

            var entityA = _context.CreateEntity();
            entityA.AddComponentA();

            var entities = _context.GetGroup(_matcherAB).GetEntities();
            entities.Length.Should().Be(2);
            entities.Should().Contain(entity1);
            entities.Should().Contain(entity2);
        }

        [Fact]
        public void GetsCachedGroup()
        {
            _context.GetGroup(_matcherAB).Should().BeSameAs(_context.GetGroup(_matcherAB));
        }

        [Fact]
        public void CachedGroupContainsNewlyCreatedMatchingEntity()
        {
            var entity = _context.CreateEntity();
            entity.AddComponentA();

            var group = _context.GetGroup(_matcherAB);
            entity.AddComponentB();
            group.GetEntities().Should().Contain(entity);
        }

        [Fact]
        public void CachedGroupDoesNotContainEntityWhichIsNotMatchingAnymore()
        {
            var entity = _context.CreateEntity();
            entity.AddComponentA();
            entity.AddComponentB();

            var group = _context.GetGroup(_matcherAB);
            entity.RemoveComponentA();
            group.GetEntities().Should().NotContain(entity);
        }

        [Fact]
        public void RemovesDestroyedEntityFromGroup()
        {
            var entity = _context.CreateEntity();
            entity.AddComponentA();
            entity.AddComponentB();

            var group = _context.GetGroup(_matcherAB);
            entity.Destroy();
            group.GetEntities().Should().NotContain(entity);
        }

        [Fact]
        public void GroupDispatchesOnEntityRemovedAndOnEntityAddedWhenReplacingComponent()
        {
            var entity = _context.CreateEntity();
            entity.AddComponentA();
            entity.AddComponentB();
            var group = _context.GetGroup(_matcherAB);
            var didDispatchRemoved = 0;
            var didDispatchAdded = 0;
            var componentA = new ComponentA();
            group.OnEntityRemoved += (g, e, index, component) =>
            {
                g.Should().BeSameAs(group);
                e.Should().BeSameAs(entity);
                index.Should().Be(CID.ComponentA);
                component.Should().BeSameAs(Component.A);
                didDispatchRemoved++;
            };
            group.OnEntityAdded += (g, e, index, component) =>
            {
                g.Should().BeSameAs(group);
                e.Should().BeSameAs(entity);
                index.Should().Be(CID.ComponentA);
                component.Should().BeSameAs(componentA);
                didDispatchAdded++;
            };
            entity.ReplaceComponentA(componentA);
            didDispatchRemoved.Should().Be(1);
            didDispatchAdded.Should().Be(1);
        }

        [Fact]
        public void GroupDispatchesOnEntityUpdatedWithPreviousAndCurrentComponentWhenReplacingComponent()
        {
            var entity = _context.CreateEntity();
            entity.AddComponentA();
            entity.AddComponentB();
            var updated = 0;
            var prevComp = entity.GetComponent(CID.ComponentA);
            var newComp = new ComponentA();
            var group = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
            group.OnEntityUpdated += (g, e, index, previousComponent, newComponent) =>
            {
                updated += 1;
                g.Should().BeSameAs(group);
                e.Should().BeSameAs(entity);
                index.Should().Be(CID.ComponentA);
                previousComponent.Should().BeSameAs(prevComp);
                newComponent.Should().BeSameAs(newComp);
            };

            entity.ReplaceComponent(CID.ComponentA, newComp);
            updated.Should().Be(1);
        }

        [Fact]
        public void GroupWithMatcherNoneOfDoesNotDispatchOnEntityAddedWhenDestroyingEntity()
        {
            var entity = _context.CreateEntity();
            entity.AddComponentA();
            entity.AddComponentB();
            var matcher = Matcher<TestEntity>.AllOf(CID.ComponentB).NoneOf(CID.ComponentA);
            var group = _context.GetGroup(matcher);
            group.OnEntityAdded += delegate { throw new Exception("group.OnEntityAdded"); };
            entity.Destroy();
        }

        [Fact]
        public void DispatchesOnEntityAddedEventsAfterAllGroupsAreUpdated()
        {
            var groupAB = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB));
            var groupB = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentB));

            groupAB.OnEntityAdded += delegate { groupB.Count.Should().Be(1); };

            var entity = _context.CreateEntity();
            entity.AddComponentA();
            entity.AddComponentB();
        }

        [Fact]
        public void DispatchesOnEntityRemovedEventsAfterAllGroupsAreUpdated()
        {
            var groupB = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentB));
            var groupAB = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB));

            groupB.OnEntityRemoved += delegate { groupAB.Count.Should().Be(0); };

            var entity = _context.CreateEntity();
            entity.AddComponentA();
            entity.AddComponentB();

            entity.RemoveComponentB();
        }

        [Fact]
        public void ThrowsWhenEntityIndexForKeyDoesNotExist()
        {
            FluentActions.Invoking(() => _context.GetEntityIndex("unknown"))
                .Should().Throw<ContextEntityIndexDoesNotExistException>();
        }

        [Fact]
        public void AddsEntityIndex()
        {
            var entityIndex = new PrimaryEntityIndex<TestEntity, string>(
                "TestIndex",
                _context.GetGroup(Matcher<TestEntity>.AllOf(1)),
                (_, _) => string.Empty
            );
            _context.AddEntityIndex(entityIndex);
            _context.GetEntityIndex(entityIndex.Name).Should().BeSameAs(entityIndex);
        }

        [Fact]
        public void ThrowsWhenAddingEntityIndexWithSameName()
        {
            var entityIndex = new PrimaryEntityIndex<TestEntity, string>(
                "TestIndex",
                _context.GetGroup(Matcher<TestEntity>.AllOf(1)),
                (_, _) => string.Empty
            );
            _context.AddEntityIndex(entityIndex);

            FluentActions.Invoking(() => _context.AddEntityIndex(entityIndex))
                .Should().Throw<ContextEntityIndexDoesAlreadyExistException>();
        }

        [Fact]
        public void ResetsCreationIndex()
        {
            _context.CreateEntity();
            _context.ResetCreationIndex();
            _context.CreateEntity().Id.Should().Be(0);
        }

        [Fact]
        public void RemovesOnEntityCreated()
        {
            _context.OnEntityCreated += delegate { throw new Exception("context.OnEntityCreated"); };
            _context.RemoveAllEventHandlers();
            _context.CreateEntity();
        }

        [Fact]
        public void RemovesOnEntityWillBeDestroyed()
        {
            _context.OnEntityWillBeDestroyed += delegate { throw new Exception("context.OnEntityWillBeDestroyed"); };
            _context.RemoveAllEventHandlers();
            _context.CreateEntity().Destroy();
        }

        [Fact]
        public void RemovesOnEntityDestroyed()
        {
            _context.OnEntityDestroyed += delegate { throw new Exception("context.OnEntityDestroyed"); };
            _context.RemoveAllEventHandlers();
            _context.CreateEntity().Destroy();
        }

        [Fact]
        public void RemovesOnGroupCreated()
        {
            _context.OnGroupCreated += delegate { throw new Exception("context.OnGroupCreated"); };
            _context.RemoveAllEventHandlers();
            _context.GetGroup(Matcher<TestEntity>.AllOf(0));
        }

        [Fact]
        public void ClearsAllComponentPools()
        {
            var entity = _context.CreateEntity();
            entity.AddComponentA();
            entity.AddComponentB();
            entity.RemoveComponentA();
            entity.RemoveComponentB();

            _context.ComponentPools[CID.ComponentA].Count.Should().Be(1);
            _context.ComponentPools[CID.ComponentB].Count.Should().Be(1);

            _context.ClearComponentPools();
            _context.ComponentPools[CID.ComponentA].Count.Should().Be(0);
            _context.ComponentPools[CID.ComponentB].Count.Should().Be(0);
        }

        [Fact]
        public void ClearsSpecificComponentPool()
        {
            var entity = _context.CreateEntity();
            entity.AddComponentA();
            entity.AddComponentB();
            entity.RemoveComponentA();
            entity.RemoveComponentB();

            _context.ClearComponentPool(CID.ComponentB);
            _context.ComponentPools[CID.ComponentA].Count.Should().Be(1);
            _context.ComponentPools[CID.ComponentB].Count.Should().Be(0);
        }

        [Fact]
        public void ThrowsWhenClearingComponentPoolThatDoesNotExist()
        {
            FluentActions.Invoking(() => _context.ClearComponentPool(99))
                .Should().Throw<IndexOutOfRangeException>();
        }

        [Fact]
        public void PopsNewListFromListPool()
        {
            var groupA = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
            var groupAB = _context.GetGroup(Matcher<TestEntity>.AnyOf(CID.ComponentA, CID.ComponentB));
            var groupABC = _context.GetGroup(Matcher<TestEntity>.AnyOf(CID.ComponentA, CID.ComponentB, CID.ComponentC));

            var didExecute = 0;

            groupA.OnEntityAdded += (_, e, _, _) =>
            {
                didExecute += 1;
                e.RemoveComponentA();
            };

            groupAB.OnEntityAdded += delegate { didExecute += 1; };
            groupABC.OnEntityAdded += delegate { didExecute += 1; };

            _context.CreateEntity().AddComponentA();

            didExecute.Should().Be(3);
        }
    }
}
