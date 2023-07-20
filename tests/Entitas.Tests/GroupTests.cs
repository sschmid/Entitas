using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class GroupTests
    {
        readonly IGroup<TestEntity> _groupA;
        readonly TestEntity _entity1;
        readonly TestEntity _entity2;

        public GroupTests()
        {
            _groupA = new Group<TestEntity>(Matcher<TestEntity>.AllOf(CID.ComponentA));
            _entity1 = CreateEntity().AddComponentA();
            _entity2 = CreateEntity().AddComponentA();
        }

        [Fact]
        public void DoesNotHaveEntitiesWhichHaveNotBeenAdded()
        {
            _groupA.GetEntities().Should().BeEmpty();
        }

        [Fact]
        public void DoesNotAddEntitiesToBuffer()
        {
            var buffer = new List<TestEntity>();
            buffer.Add(CreateEntity());
            var retBuffer = _groupA.GetEntities(buffer);
            buffer.Should().BeEmpty();
            retBuffer.Should().BeSameAs(buffer);
        }

        [Fact]
        public void IsEmpty()
        {
            _groupA.count.Should().Be(0);
        }

        [Fact]
        public void DoesNotContainEntity()
        {
            _groupA.ContainsEntity(_entity1).Should().BeFalse();
        }

        [Fact]
        public void AddsMatchingEntity()
        {
            HandleSilently(_entity1);
            AssertContains(_entity1);
        }

        [Fact]
        public void FillsBufferWithEntities()
        {
            HandleSilently(_entity1);
            var buffer = new List<TestEntity>();
            _groupA.GetEntities(buffer);
            buffer.Count.Should().Be(1);
            buffer[0].Should().BeSameAs(_entity1);
        }

        [Fact]
        public void ClearsBufferBeforeFilling()
        {
            HandleSilently(_entity1);
            var buffer = new List<TestEntity>();
            buffer.Add(CreateEntity());
            buffer.Add(CreateEntity());
            _groupA.GetEntities(buffer);
            buffer.Count.Should().Be(1);
            buffer[0].Should().BeSameAs(_entity1);
        }

        [Fact]
        public void DoesNotAddSameEntityTwice()
        {
            HandleSilently(_entity1);
            HandleSilently(_entity1);
            AssertContains(_entity1);
        }

        [Fact]
        public void EnumeratesGroup()
        {
            HandleSilently(_entity1);
            var i = 0;
            IEntity e = null;
            foreach (var entity in _groupA)
            {
                i++;
                e = entity;
            }

            i.Should().Be(1);
            e.Should().BeSameAs(_entity1);
        }

        [Fact]
        public void ReturnsEnumerable()
        {
            HandleSilently(_entity1);
            _groupA.AsEnumerable().Single().Should().BeSameAs(_entity1);
        }

        [Fact]
        public void RemovesEntity()
        {
            HandleSilently(_entity1);
            _entity1.RemoveComponentA();
            HandleSilently(_entity1);
            AssertContainsNot(_entity1);
        }

        [Fact]
        public void DoesNotAddEntityWhenEntityIsNotEnabled()
        {
            _entity1.InternalDestroy();
            HandleSilently(_entity1);
            AssertContainsNot(_entity1);
        }

        [Fact]
        public void DoesNotAddEntityWhenNotMatching()
        {
            var e = CreateEntity().AddComponentB();
            HandleSilently(e);
            AssertContainsNot(e);
        }

        [Fact]
        public void ReturnsNullWhenSingleEntityDoesNotExist()
        {
            _groupA.GetSingleEntity().Should().BeNull();
        }

        [Fact]
        public void ReturnsSingleEntity()
        {
            HandleSilently(_entity1);
            _groupA.GetSingleEntity().Should().BeSameAs(_entity1);
        }

        [Fact]
        public void ThrowsWhenGettingSingleEntityAndMultipleMatchingEntitiesExist()
        {
            HandleSilently(_entity1);
            HandleSilently(_entity2);
            FluentActions.Invoking(() => _groupA.GetSingleEntity())
                .Should().Throw<GroupSingleEntityException<TestEntity>>();
        }

        [Fact]
        public void DispatchesOnEntityAddedWhenMatchingEntityAdded()
        {
            var didDispatch = 0;
            _groupA.OnEntityAdded += (group, entity, index, component) =>
            {
                didDispatch++;
                group.Should().BeSameAs(_groupA);
                entity.Should().BeSameAs(_entity1);
                index.Should().Be(CID.ComponentA);
                component.Should().BeSameAs(Component.A);
            };
            _groupA.OnEntityRemoved += delegate { throw new Exception("group.OnEntityRemoved"); };
            _groupA.OnEntityUpdated += delegate { throw new Exception("group.OnEntityUpdated"); };

            HandleAddA(_entity1);
            didDispatch.Should().Be(1);
        }

        [Fact]
        public void DoesNotDispatchOnEntityAddedWhenMatchingEntityAlreadyHasBeenAdded()
        {
            HandleAddA(_entity1);
            _groupA.OnEntityAdded += delegate { throw new Exception("group.OnEntityAdded"); };
            _groupA.OnEntityRemoved += delegate { throw new Exception("group.OnEntityRemoved"); };
            _groupA.OnEntityUpdated += delegate { throw new Exception("group.OnEntityUpdated"); };
            HandleAddA(_entity1);
        }

        [Fact]
        public void DoesNotDispatchOnEntityAddedWhenEntityIsNotMatching()
        {
            var e = CreateEntity().AddComponentB();
            _groupA.OnEntityAdded += delegate { throw new Exception("group.OnEntityAdded"); };
            _groupA.OnEntityRemoved += delegate { throw new Exception("group.OnEntityRemoved"); };
            _groupA.OnEntityUpdated += delegate { throw new Exception("group.OnEntityUpdated"); };
            HandleAddB(e);
        }

        [Fact]
        public void DispatchesOnEntityRemovedWhenEntityGotRemoved()
        {
            var didDispatch = 0;
            HandleSilently(_entity1);
            _groupA.OnEntityRemoved += (group, entity, index, component) =>
            {
                didDispatch++;
                group.Should().BeSameAs(_groupA);
                entity.Should().BeSameAs(_entity1);
                index.Should().Be(CID.ComponentA);
                component.Should().BeSameAs(Component.A);
            };
            _groupA.OnEntityAdded += delegate { throw new Exception("group.OnEntityAdded"); };
            _groupA.OnEntityUpdated += delegate { throw new Exception("group.OnEntityUpdated"); };

            _entity1.RemoveComponentA();
            HandleRemoveA(_entity1, Component.A);

            didDispatch.Should().Be(1);
        }

        [Fact]
        public void DoesNotDispatchOnEntityRemovedWhenEntityDidNotGetRemoved()
        {
            _groupA.OnEntityRemoved += delegate { throw new Exception("group.OnEntityRemoved"); };
            _entity1.RemoveComponentA();
            HandleRemoveA(_entity1, Component.A);
        }

        [Fact]
        public void DispatchesOnEntityRemovedOnEntityAddedAndOnEntityUpdatedWhenUpdating()
        {
            HandleSilently(_entity1);

            var removed = 0;
            var added = 0;
            var updated = 0;
            var newComponentA = new ComponentA();

            _groupA.OnEntityRemoved += (group, entity, index, component) =>
            {
                removed += 1;
                group.Should().Be(_groupA);
                entity.Should().Be(_entity1);
                index.Should().Be(CID.ComponentA);
                component.Should().BeSameAs(Component.A);
            };
            _groupA.OnEntityAdded += (group, entity, index, component) =>
            {
                added += 1;
                group.Should().Be(_groupA);
                entity.Should().Be(_entity1);
                index.Should().Be(CID.ComponentA);
                component.Should().BeSameAs(newComponentA);
            };
            _groupA.OnEntityUpdated += (group, entity, index, previousComponent, newComponent) =>
            {
                updated += 1;
                group.Should().Be(_groupA);
                entity.Should().Be(_entity1);
                index.Should().Be(CID.ComponentA);
                previousComponent.Should().BeSameAs(Component.A);
                newComponent.Should().BeSameAs(newComponentA);
            };

            UpdateA(_entity1, newComponentA);

            removed.Should().Be(1);
            added.Should().Be(1);
            updated.Should().Be(1);
        }

        [Fact]
        public void DoesNotDispatchOnEntityRemovedAndOnEntityAddedWhenUpdatingWhenGroupDoesNotContainEntity()
        {
            _groupA.OnEntityRemoved += delegate { throw new Exception("group.OnEntityRemoved"); };
            _groupA.OnEntityAdded += delegate { throw new Exception("group.OnEntityAdded"); };
            _groupA.OnEntityUpdated += delegate { throw new Exception("group.OnEntityUpdated"); };
            UpdateA(_entity1, new ComponentA());
        }

        [Fact]
        public void RemovesAllEventHandlers()
        {
            _groupA.OnEntityAdded += delegate { throw new Exception("group.OnEntityAdded"); };
            _groupA.OnEntityRemoved += delegate { throw new Exception("group.OnEntityRemoved"); };
            _groupA.OnEntityUpdated += delegate { throw new Exception("group.OnEntityUpdated"); };

            _groupA.RemoveAllEventHandlers();

            HandleAddA(_entity1);

            var cA = _entity1.GetComponentA();
            _entity1.RemoveComponentA();
            HandleRemoveA(_entity1, cA);

            _entity1.AddComponentA();
            HandleAddA(_entity1);
            UpdateA(_entity1, Component.A);
        }

        [Fact]
        public void GetsCachedEntities()
        {
            HandleSilently(_entity1);
            _groupA.GetEntities().Should().BeSameAs(_groupA.GetEntities());
        }

        [Fact]
        public void UpdatesCacheWhenAddingNewMatchingEntity()
        {
            HandleSilently(_entity1);
            var cache = _groupA.GetEntities();
            HandleSilently(_entity2);
            _groupA.GetEntities().Should().NotBeSameAs(cache);
        }

        [Fact]
        public void DoesNotUpdateCacheWhenAddingNotMatchingEntity()
        {
            HandleSilently(_entity1);
            var cache = _groupA.GetEntities();
            var e = CreateEntity();
            HandleSilently(e);
            _groupA.GetEntities().Should().BeSameAs(cache);
        }

        [Fact]
        public void UpdatesCacheWhenRemovingEntity()
        {
            HandleSilently(_entity1);
            var cache = _groupA.GetEntities();
            _entity1.RemoveComponentA();
            HandleSilently(_entity1);
            _groupA.GetEntities().Should().NotBeSameAs(cache);
        }

        [Fact]
        public void DoesNotUpdateCacheWhenRemovingEntityThatWasNotAddedBefore()
        {
            HandleSilently(_entity1);
            var cache = _groupA.GetEntities();
            _entity2.RemoveComponentA();
            HandleSilently(_entity2);
            _groupA.GetEntities().Should().BeSameAs(cache);
        }

        [Fact]
        public void DoesNotUpdateCacheWhenUpdatingEntity()
        {
            HandleSilently(_entity1);
            var cache = _groupA.GetEntities();
            UpdateA(_entity1, new ComponentA());
            _groupA.GetEntities().Should().BeSameAs(cache);
        }

        [Fact]
        public void GetsCachedSingleEntities()
        {
            HandleSilently(_entity1);
            var cache = _groupA.GetSingleEntity();
            _groupA.GetSingleEntity().Should().BeSameAs(cache);
        }

        [Fact]
        public void UpdatesCacheWhenNewSingleEntityIsAdded()
        {
            HandleSilently(_entity1);
            var cache = _groupA.GetSingleEntity();
            _entity1.RemoveComponentA();
            HandleSilently(_entity1);
            HandleSilently(_entity2);
            _groupA.GetSingleEntity().Should().NotBeSameAs(cache);
        }

        [Fact]
        public void UpdatesCacheWhenSingleEntityIsRemoved()
        {
            HandleSilently(_entity1);
            var cache = _groupA.GetSingleEntity();
            _entity1.RemoveComponentA();
            HandleSilently(_entity1);
            _groupA.GetSingleEntity().Should().NotBeSameAs(cache);
        }

        [Fact]
        public void DoesNotUpdateCacheWhenSingleEntityIsUpdated()
        {
            HandleSilently(_entity1);
            var cache = _groupA.GetSingleEntity();
            UpdateA(_entity1, new ComponentA());
            _groupA.GetSingleEntity().Should().BeSameAs(cache);
        }

        [Fact]
        public void RetainsMatchedEntity()
        {
            _entity1.retainCount.Should().Be(0);
            HandleSilently(_entity1);
            _entity1.retainCount.Should().Be(1);
        }

        [Fact]
        public void ReleasesRemovedEntity()
        {
            HandleSilently(_entity1);
            _entity1.RemoveComponentA();
            HandleSilently(_entity1);
            _entity1.retainCount.Should().Be(0);
        }

        [Fact]
        public void UpdatesCacheBeforeCallingDelegatesSilently()
        {
            var didExecute = 0;
            _entity1.OnEntityReleased += _ =>
            {
                didExecute += 1;
                _groupA.GetEntities().Length.Should().Be(0);
            };
            HandleSilently(_entity1);
            _groupA.GetEntities();
            _entity1.RemoveComponentA();
            HandleSilently(_entity1);
            didExecute.Should().Be(1);
        }

        [Fact]
        public void UpdatesCacheBeforeCallingDelegates()
        {
            var didExecute = 0;
            _entity1.OnEntityReleased += _ =>
            {
                didExecute += 1;
                _groupA.GetEntities().Length.Should().Be(0);
            };
            HandleAddA(_entity1);
            _groupA.GetEntities();
            _entity1.RemoveComponentA();
            HandleRemoveA(_entity1, Component.A);
            didExecute.Should().Be(1);
        }

        [Fact]
        public void UpdatesSingleEntityCacheBeforeCallingDelegatesSilently()
        {
            var didExecute = 0;
            _entity1.OnEntityReleased += _ =>
            {
                didExecute += 1;
                _groupA.GetSingleEntity().Should().BeNull();
            };
            HandleSilently(_entity1);
            _groupA.GetSingleEntity();
            _entity1.RemoveComponentA();
            HandleSilently(_entity1);
            didExecute.Should().Be(1);
        }

        [Fact]
        public void UpdatesSingleEntityCacheBeforeCallingDelegates()
        {
            var didExecute = 0;
            _entity1.OnEntityReleased += _ =>
            {
                didExecute += 1;
                _groupA.GetSingleEntity().Should().BeNull();
            };
            HandleAddA(_entity1);
            _groupA.GetSingleEntity();
            _entity1.RemoveComponentA();
            HandleRemoveA(_entity1, Component.A);
            didExecute.Should().Be(1);
        }

        [Fact]
        public void RetainsEntityUntilAfterEventHandlersWereCalled()
        {
            HandleAddA(_entity1);
            var didDispatch = 0;
            _groupA.OnEntityRemoved += (_, entity, _, _) =>
            {
                didDispatch += 1;
                entity.retainCount.Should().Be(1);
            };
            _entity1.RemoveComponentA();
            HandleRemoveA(_entity1, Component.A);

            didDispatch.Should().Be(1);
            _entity1.retainCount.Should().Be(0);
        }

        [Fact]
        public void CanToString()
        {
            var matcher = Matcher<TestEntity>.AllOf(Matcher<TestEntity>.AllOf(0), Matcher<TestEntity>.AllOf(1));
            var group = new Group<TestEntity>(matcher);
            group.ToString().Should().Be("Group(AllOf(0, 1))");
        }

        public static TestEntity CreateEntity()
        {
            var entity = new TestEntity();
            entity.Initialize(0, CID.TotalComponents, new Stack<IComponent>[CID.TotalComponents]);
            return entity;
        }

        void AssertContains(params TestEntity[] expectedEntities)
        {
            _groupA.count.Should().Be(expectedEntities.Length);

            var entities = _groupA.GetEntities();
            entities.Length.Should().Be(expectedEntities.Length);

            foreach (var e in expectedEntities)
            {
                entities.Should().Contain(e);
                _groupA.ContainsEntity(e).Should().BeTrue();
            }
        }

        void AssertContainsNot(TestEntity entity)
        {
            _groupA.count.Should().Be(0);
            _groupA.GetEntities().Should().BeEmpty();
            _groupA.ContainsEntity(entity).Should().BeFalse();
        }

        void HandleSilently(TestEntity entity) => _groupA.HandleEntitySilently(entity);
        void Handle(TestEntity entity, int index, IComponent component) => _groupA.HandleEntity(entity, index, component);
        void HandleAddA(TestEntity entity) => Handle(entity, CID.ComponentA, entity.GetComponentA());
        void HandleAddB(TestEntity entity) => Handle(entity, CID.ComponentB, entity.GetComponentB());
        void HandleRemoveA(TestEntity entity, IComponent component) => Handle(entity, CID.ComponentA, component);
        void UpdateA(TestEntity entity, IComponent component) => _groupA.UpdateEntity(entity, CID.ComponentA, Component.A, component);
    }
}
