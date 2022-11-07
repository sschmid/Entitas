using System;
using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class ContextTests
    {
        readonly IContext<TestEntity> _context;
        readonly IContext<TestEntity> _contextWithInfo;
        readonly ContextInfo _contextInfo;
        readonly IAllOfMatcher<TestEntity> _matcherAB = Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB);

        public ContextTests()
        {
            _context = new MyTestContext();
            var componentNames = new[] {"Health", "Position", "View"};
            var componentTypes = new[] {typeof(ComponentA), typeof(ComponentB), typeof(ComponentC)};
            _contextInfo = new ContextInfo("My Context", componentNames, componentTypes);
            _contextWithInfo = new MyTestContext(componentNames.Length, 0, _contextInfo);
        }

        [Fact]
        public void IncrementsCreationIndex()
        {
            _context.CreateEntity().creationIndex.Should().Be(0);
            _context.CreateEntity().creationIndex.Should().Be(1);
        }

        [Fact]
        public void StartsWithGivenCreationIndex()
        {
            new MyTestContext(CID.TotalComponents, 42, null)
                .CreateEntity().creationIndex.Should().Be(42);
        }

        [Fact]
        public void IsEmpty()
        {
            _context.GetEntities().Should().BeEmpty();
        }

        [Fact]
        public void TotalEntityCountIsZero()
        {
            _context.count.Should().Be(0);
        }

        [Fact]
        public void CreatesEntity()
        {
            var e = _context.CreateEntity();
            e.Should().NotBeNull();
            e.GetType().Should().Be(typeof(TestEntity));
            e.totalComponents.Should().Be(_context.totalComponents);
            e.isEnabled.Should().BeTrue();
        }

        [Fact]
        public void HasDefaultContextInfo()
        {
            _context.contextInfo.name.Should().Be("Unnamed Context");
            _context.contextInfo.componentNames.Length.Should().Be(CID.TotalComponents);
            for (var i = 0; i < _context.contextInfo.componentNames.Length; i++)
                _context.contextInfo.componentNames[i].Should().Be($"Index {i}");
        }

        [Fact]
        public void CreatesComponentPools()
        {
            _context.componentPools.Should().NotBeNull();
            _context.componentPools.Length.Should().Be(CID.TotalComponents);
        }

        [Fact]
        public void CreatesEntityWithComponentPools()
        {
            _context.CreateEntity().componentPools
                .Should().BeSameAs(_context.componentPools);
        }

        [Fact]
        public void CanToString()
        {
            _context.ToString().Should().Be("Unnamed Context");
        }

        [Fact]
        public void HasCustomContextInfo()
        {
            _contextWithInfo.contextInfo.Should().BeSameAs(_contextInfo);
        }

        [Fact]
        public void CreatesEntityWithSameContextInfo()
        {
            _contextWithInfo.CreateEntity().contextInfo.Should().BeSameAs(_contextInfo);
        }

        [Fact]
        public void ThrowsWhenComponentNamesLengthIsNotEqualToTotalComponents()
        {
            FluentActions.Invoking(() =>
                new MyTestContext(_contextInfo.componentNames.Length + 1, 0, _contextInfo)
            ).Should().Throw<ContextInfoException>();
        }

        [Fact]
        public void GetsTotalEntityCount()
        {
            _context.CreateEntity().AddComponentA();
            _context.count.Should().Be(1);
        }

        [Fact]
        public void HasEntitiesThatWereCreatedWithCreateEntity()
        {
            var e = _context.CreateEntity().AddComponentA();
            _context.HasEntity(e).Should().BeTrue();
        }

        [Fact]
        public void ReturnsAllCreatedEntities()
        {
            var e1 = _context.CreateEntity();
            var e2 = _context.CreateEntity();
            var entities = _context.GetEntities();
            entities.Length.Should().Be(2);
            entities.Should().Contain(e1);
            entities.Should().Contain(e2);
        }

        [Fact]
        public void DestroysEntityAndRemovesIt()
        {
            var e = _context.CreateEntity();
            e.Destroy();
            _context.HasEntity(e).Should().BeFalse();
            _context.count.Should().Be(0);
            _context.GetEntities().Should().BeEmpty();
        }

        [Fact]
        public void DestroysEntityAndRemovesAllComponents()
        {
            var e = _context.CreateEntity();
            e.Destroy();
            e.GetComponents().Should().BeEmpty();
        }

        [Fact]
        public void RemovesOnDestroyEntityHandler()
        {
            var e = _context.CreateEntity();
            var didDestroy = 0;
            _context.OnEntityWillBeDestroyed += delegate { didDestroy += 1; };
            e.Destroy();
            _context.CreateEntity().Destroy();
            didDestroy.Should().Be(2);
        }

        [Fact]
        public void DestroysAllEntities()
        {
            var e = _context.CreateEntity();
            _context.CreateEntity();
            _context.DestroyAllEntities();
            _context.HasEntity(e).Should().BeFalse();
            _context.count.Should().Be(0);
            _context.GetEntities().Should().BeEmpty();
            e.GetComponents().Should().BeEmpty();
        }

        [Fact]
        public void EnsuresSameDeterministicOrderWhenGettingEntitiesAfterDestroyingAllEntities()
        {
            // This is a Unity specific problem. Run Unity Test Tools with in the Unity project

            const int numEntities = 10;

            for (var i = 0; i < numEntities; i++)
                _context.CreateEntity();

            var order1 = new int[numEntities];
            var entities1 = _context.GetEntities();
            for (var i = 0; i < numEntities; i++)
                order1[i] = entities1[i].creationIndex;

            _context.DestroyAllEntities();
            _context.ResetCreationIndex();

            for (var i = 0; i < numEntities; i++)
                _context.CreateEntity();

            var order2 = new int[numEntities];
            var entities2 = _context.GetEntities();
            for (var i = 0; i < numEntities; i++)
                order2[i] = entities2[i].creationIndex;

            for (var i = 0; i < numEntities; i++)
                order1[i].Should().Be(order2[i]);
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
            var entities = _context.GetEntities();
            _context.GetEntities().Should().BeSameAs(entities);
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
            var e = _context.CreateEntity();
            var entities = _context.GetEntities();
            e.Destroy();
            _context.GetEntities().Should().NotBeSameAs(entities);
        }

        [Fact]
        public void DispatchesOnEntityCreatedWhenCreatingNewEntity()
        {
            var didDispatch = 0;
            IEntity eventEntity = null;
            _context.OnEntityCreated += (c, entity) =>
            {
                didDispatch += 1;
                eventEntity = entity;
                c.Should().BeSameAs(_context);
            };

            var e = _context.CreateEntity();
            didDispatch.Should().Be(1);
            eventEntity.Should().BeSameAs(e);
        }

        [Fact]
        public void DispatchesOnEntityWillBeDestroyedWhenDestroyingEntity()
        {
            var e = _context.CreateEntity().AddComponentA();
            var didDispatch = 0;
            _context.OnEntityWillBeDestroyed += (c, entity) =>
            {
                didDispatch += 1;
                c.Should().BeSameAs(_context);
                entity.Should().BeSameAs(e);
                entity.HasComponentA().Should().BeTrue();
                entity.isEnabled.Should().BeTrue();

                ((IContext<TestEntity>)c).GetEntities().Length.Should().Be(0);
            };
            _context.GetEntities();
            e.Destroy();
            didDispatch.Should().Be(1);
        }

        [Fact]
        public void DispatchesOnEntityDestroyedWhenDestroyingEntity()
        {
            var e = _context.CreateEntity();
            var didDispatch = 0;
            _context.OnEntityDestroyed += (p, entity) =>
            {
                didDispatch += 1;
                p.Should().BeSameAs(_context);
                entity.Should().BeSameAs(e);
                entity.HasComponentA().Should().BeFalse();
                entity.isEnabled.Should().BeFalse();
            };
            e.Destroy();
            didDispatch.Should().Be(1);
        }

        [Fact]
        public void ReleasesEntityAfterOnEntityDestroyed()
        {
            var e = _context.CreateEntity();
            var didDispatch = 0;
            _context.OnEntityDestroyed += (_, entity) =>
            {
                didDispatch += 1;
                entity.retainCount.Should().Be(1);
                var newEntity = _context.CreateEntity();
                newEntity.Should().NotBeNull();
                newEntity.Should().NotBeSameAs(entity);
            };
            e.Destroy();
            var reusedEntity = _context.CreateEntity();
            reusedEntity.Should().BeSameAs(e);
            didDispatch.Should().Be(1);
        }

        [Fact]
        public void ThrowsIfEntityIsReleasedBeforeItIsDestroyed()
        {
            FluentActions.Invoking(() =>
                    _context.CreateEntity().Release(_context))
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
            var e = _context.CreateEntity();
            e.OnComponentAdded += delegate { throw new Exception("entity.OnComponentAdded"); };
            e.OnComponentRemoved += delegate { throw new Exception("entity.OnComponentRemoved"); };
            e.OnComponentReplaced += delegate { throw new Exception("entity.OnComponentReplaced"); };
            e.Destroy();
            var e2 = _context.CreateEntity();
            e2.Should().BeSameAs(e);
            e2.AddComponentA();
            e2.ReplaceComponentA(Component.A);
            e2.RemoveComponentA();
        }

        [Fact]
        public void WillNotRemoveOnEntityReleased()
        {
            var e = _context.CreateEntity();
            var didRelease = 0;
            e.OnEntityReleased += delegate { didRelease += 1; };
            e.Destroy();
            didRelease.Should().Be(1);
        }

        [Fact]
        public void RemovesOnEntityReleasedAfterBeingDispatched()
        {
            var e = _context.CreateEntity();
            var didRelease = 0;
            e.OnEntityReleased += delegate { didRelease += 1; };
            e.Destroy();
            e.Retain(this);
            e.Release(this);
            didRelease.Should().Be(1);
        }

        [Fact]
        public void RemovesOnEntityReleasedAfterBeingDispatchedWhenDelayedRelease()
        {
            var e = _context.CreateEntity();
            var didRelease = 0;
            e.OnEntityReleased += delegate { didRelease += 1; };
            e.Retain(this);
            e.Destroy();
            didRelease.Should().Be(0);
            e.Release(this);
            didRelease.Should().Be(1);

            e.Retain(this);
            e.Release(this);
            didRelease.Should().Be(1);
        }

        [Fact]
        public void GetsEntityFromObjectPool()
        {
            var e = _context.CreateEntity();
            e.Should().NotBeNull();
            e.GetType().Should().Be(typeof(TestEntity));
        }

        [Fact]
        public void DestroysEntityWhenPushingBackToObjectPool()
        {
            var e = _context.CreateEntity().AddComponentA();
            e.Destroy();
            e.HasComponent(CID.ComponentA).Should().BeFalse();
        }

        [Fact]
        public void ReturnsPushedEntity()
        {
            var e = _context.CreateEntity().AddComponentA();
            e.Destroy();
            var entity = _context.CreateEntity();
            entity.HasComponent(CID.ComponentA).Should().BeFalse();
            entity.Should().BeSameAs(e);
        }

        [Fact]
        public void OnlyReturnsReleasedEntities()
        {
            var e1 = _context.CreateEntity();
            e1.Retain(this);
            e1.Destroy();
            var e2 = _context.CreateEntity();
            e2.Should().NotBeSameAs(e1);
            e1.Release(this);
            var e3 = _context.CreateEntity();
            e3.Should().BeSameAs(e1);
        }

        [Fact]
        public void ReturnsNewEntity()
        {
            var e1 = _context.CreateEntity().AddComponentA();
            e1.Destroy();
            _context.CreateEntity();
            var e2 = _context.CreateEntity();
            e2.HasComponent(CID.ComponentA).Should().BeFalse();
            e2.Should().NotBeSameAs(e1);
        }

        [Fact]
        public void SetsUpEntityFromObjectPool()
        {
            var e = _context.CreateEntity();
            var creationIndex = e.creationIndex;
            e.Destroy();
            var g = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));

            e = _context.CreateEntity();
            e.creationIndex.Should().Be(creationIndex + 1);
            e.isEnabled.Should().BeTrue();

            e.AddComponentA();
            g.GetEntities().Should().Contain(e);
        }

        [Fact]
        public void ThrowsWhenAddingComponentToDestroyedEntity()
        {
            var e = _context.CreateEntity().AddComponentA();
            e.Destroy();
            FluentActions.Invoking(() => e.AddComponentA())
                .Should().Throw<EntityIsNotEnabledException>();
        }

        [Fact]
        public void ThrowsWhenRemovingComponentFromDestroyedEntity()
        {
            var e = _context.CreateEntity().AddComponentA();
            e.Destroy();
            FluentActions.Invoking(() => e.RemoveComponentA())
                .Should().Throw<EntityIsNotEnabledException>();
        }

        [Fact]
        public void ThrowsWhenReplacingComponentOnDestroyedEntity()
        {
            var e = _context.CreateEntity().AddComponentA();
            e.Destroy();
            FluentActions.Invoking(() => e.ReplaceComponentA(new ComponentA()))
                .Should().Throw<EntityIsNotEnabledException>();
        }

        [Fact]
        public void ThrowsWhenReplacingComponentWithNullOnDestroyedEntity()
        {
            var e = _context.CreateEntity().AddComponentA();
            e.Destroy();
            FluentActions.Invoking(() => e.ReplaceComponentA(null))
                .Should().Throw<EntityIsNotEnabledException>();
        }

        [Fact]
        public void ThrowsWhenDestroyingDestroyedEntity()
        {
            var e = _context.CreateEntity().AddComponentA();
            e.Destroy();
            FluentActions.Invoking(() => e.Destroy())
                .Should().Throw<EntityIsNotEnabledException>();
        }

        [Fact]
        public void GetsEmptyGroupForMatcherWhenNoEntitiesWereCreated()
        {
            var g = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
            g.Should().NotBeNull();
            g.GetEntities().Should().BeEmpty();
        }

        [Fact]
        public void GetsGroupWithMatchingEntities()
        {
            var e1 = _context.CreateEntity();
            e1.AddComponentA();
            e1.AddComponentB();

            var e2 = _context.CreateEntity();
            e2.AddComponentA();
            e2.AddComponentB();

            var eA = _context.CreateEntity();
            eA.AddComponentA();

            var g = _context.GetGroup(_matcherAB).GetEntities();
            g.Length.Should().Be(2);
            g.Should().Contain(e1);
            g.Should().Contain(e2);
        }

        [Fact]
        public void GetsCachedGroup()
        {
            _context.GetGroup(_matcherAB).Should().BeSameAs(_context.GetGroup(_matcherAB));
        }

        [Fact]
        public void CachedGroupContainsNewlyCreatedMatchingEntity()
        {
            var e = _context.CreateEntity();
            e.AddComponentA();
            var g = _context.GetGroup(_matcherAB);
            e.AddComponentB();
            g.GetEntities().Should().Contain(e);
        }

        [Fact]
        public void CachedGroupDoesNotContainEntityWhichIsNotMatchingAnymore()
        {
            var e = _context.CreateEntity();
            e.AddComponentA();
            e.AddComponentB();
            var g = _context.GetGroup(_matcherAB);
            e.RemoveComponentA();
            g.GetEntities().Should().NotContain(e);
        }

        [Fact]
        public void RemovesDestroyedEntityFromGroup()
        {
            var e = _context.CreateEntity();
            e.AddComponentA();
            e.AddComponentB();
            var g = _context.GetGroup(_matcherAB);
            e.Destroy();
            g.GetEntities().Should().NotContain(e);
        }

        [Fact]
        public void GroupDispatchesOnEntityRemovedAndOnEntityAddedWhenReplacingComponent()
        {
            var e = _context.CreateEntity();
            e.AddComponentA();
            e.AddComponentB();
            var g = _context.GetGroup(_matcherAB);
            var didDispatchRemoved = 0;
            var didDispatchAdded = 0;
            var componentA = new ComponentA();
            g.OnEntityRemoved += (group, entity, index, component) =>
            {
                group.Should().BeSameAs(g);
                entity.Should().BeSameAs(e);
                index.Should().Be(CID.ComponentA);
                component.Should().BeSameAs(Component.A);
                didDispatchRemoved++;
            };
            g.OnEntityAdded += (group, entity, index, component) =>
            {
                group.Should().BeSameAs(g);
                entity.Should().BeSameAs(e);
                index.Should().Be(CID.ComponentA);
                component.Should().BeSameAs(componentA);
                didDispatchAdded++;
            };
            e.ReplaceComponentA(componentA);
            didDispatchRemoved.Should().Be(1);
            didDispatchAdded.Should().Be(1);
        }

        [Fact]
        public void GroupDispatchesOnEntityUpdatedWithPreviousAndCurrentComponentWhenReplacingComponent()
        {
            var e = _context.CreateEntity();
            e.AddComponentA();
            e.AddComponentB();
            var updated = 0;
            var prevComp = e.GetComponent(CID.ComponentA);
            var newComp = new ComponentA();
            var g = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
            g.OnEntityUpdated += (group, entity, index, previousComponent, newComponent) =>
            {
                updated += 1;
                group.Should().BeSameAs(g);
                entity.Should().BeSameAs(e);
                index.Should().Be(CID.ComponentA);
                previousComponent.Should().BeSameAs(prevComp);
                newComponent.Should().BeSameAs(newComp);
            };

            e.ReplaceComponent(CID.ComponentA, newComp);
            updated.Should().Be(1);
        }

        [Fact]
        public void GroupWithMatcherNoneOfDoesNotDispatchOnEntityAddedWhenDestroyingEntity()
        {
            var e = _context.CreateEntity();
            e.AddComponentA();
            e.AddComponentB();
            var matcher = Matcher<TestEntity>.AllOf(CID.ComponentB).NoneOf(CID.ComponentA);
            var g = _context.GetGroup(matcher);
            g.OnEntityAdded += delegate { throw new Exception("group.OnEntityAdded"); };
            e.Destroy();
        }

        [Fact]
        public void DispatchesOnEntityAddedEventsAfterAllGroupsAreUpdated()
        {
            var groupAB = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB));
            var groupB = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentB));

            groupAB.OnEntityAdded += delegate { groupB.count.Should().Be(1); };

            var entity = _context.CreateEntity();
            entity.AddComponentA();
            entity.AddComponentB();
        }

        [Fact]
        public void DispatchesOnEntityRemovedEventsAfterAllGroupsAreUpdated()
        {
            var groupB = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentB));
            var groupAB = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB));

            groupB.OnEntityRemoved += delegate { groupAB.count.Should().Be(0); };

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
            _context.GetEntityIndex(entityIndex.name).Should().BeSameAs(entityIndex);
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
            _context.CreateEntity().creationIndex.Should().Be(0);
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

            _context.componentPools[CID.ComponentA].Count.Should().Be(1);
            _context.componentPools[CID.ComponentB].Count.Should().Be(1);
            _context.ClearComponentPools();
            _context.componentPools[CID.ComponentA].Count.Should().Be(0);
            _context.componentPools[CID.ComponentB].Count.Should().Be(0);
        }

        [Fact]
        public void ClearsSpecificComponentpool()
        {
            var entity = _context.CreateEntity();
            entity.AddComponentA();
            entity.AddComponentB();
            entity.RemoveComponentA();
            entity.RemoveComponentB();

            _context.ClearComponentPool(CID.ComponentB);
            _context.componentPools[CID.ComponentA].Count.Should().Be(1);
            _context.componentPools[CID.ComponentB].Count.Should().Be(0);
        }

        [Fact]
        public void Fails()
        {
            _context.ClearComponentPool(CID.ComponentC);
        }

        [Fact]
        public void PopsNewListFromListPool()
        {
            var groupA = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
            var groupAB = _context.GetGroup(Matcher<TestEntity>.AnyOf(CID.ComponentA, CID.ComponentB));
            var groupABC = _context.GetGroup(Matcher<TestEntity>.AnyOf(CID.ComponentA, CID.ComponentB, CID.ComponentC));

            var didExecute = 0;

            groupA.OnEntityAdded += (_, entity, _, _) =>
            {
                didExecute += 1;
                entity.RemoveComponentA();
            };

            groupAB.OnEntityAdded += delegate { didExecute += 1; };
            groupABC.OnEntityAdded += delegate { didExecute += 1; };

            _context.CreateEntity().AddComponentA();

            didExecute.Should().Be(3);
        }
    }
}
