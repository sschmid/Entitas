using System;
using System.Collections.Generic;
using FluentAssertions;
using My.Namespace;
using Xunit;

namespace Entitas.Tests
{
    public class EntityTests
    {
        readonly int[] _indicesA = {CID.ComponentA};
        readonly int[] _indicesAB = {CID.ComponentA, CID.ComponentB};
        readonly TestEntity _entity;

        public EntityTests()
        {
            _entity = new TestEntity();
            _entity.Initialize(0, CID.TotalComponents, new Stack<IComponent>[CID.TotalComponents]);
        }

        [Fact]
        public void HasDefaultContextInfo()
        {
            _entity.contextInfo.name.Should().Be("No Context");
            _entity.contextInfo.componentNames.Length.Should().Be(CID.TotalComponents);
            _entity.contextInfo.componentTypes.Should().BeNull();
            for (var i = 0; i < _entity.contextInfo.componentNames.Length; i++)
                _entity.contextInfo.componentNames[i].Should().Be(i.ToString());
        }

        [Fact]
        public void InitializesEntity()
        {
            var contextInfo = new ContextInfo(null, null, null);
            var componentPools = new Stack<IComponent>[42];
            var entity = new TestEntity();
            entity.Initialize(1, 2, componentPools, contextInfo);

            entity.isEnabled.Should().BeTrue();
            entity.creationIndex.Should().Be(1);
            entity.totalComponents.Should().Be(2);
            entity.componentPools.Should().BeSameAs(componentPools);
            entity.contextInfo.Should().BeSameAs(contextInfo);
        }

        [Fact]
        public void ReactivatesEntityAfterBeingDestroyed()
        {
            var contextInfo = new ContextInfo(null, null, null);
            var componentPools = new Stack<IComponent>[42];
            var entity = new TestEntity();
            entity.Initialize(1, 2, componentPools, contextInfo);

            entity.InternalDestroy();

            entity.Reactivate(42);

            entity.isEnabled.Should().BeTrue();
            entity.creationIndex.Should().Be(42);
            entity.totalComponents.Should().Be(2);
            entity.componentPools.Should().BeSameAs(componentPools);
            entity.contextInfo.Should().BeSameAs(contextInfo);
        }

        [Fact]
        public void ThrowsWhenGettingComponentThatDoesNotExist()
        {
            FluentActions.Invoking(() => _entity.GetComponentA())
                .Should().Throw<EntityDoesNotHaveComponentException>();
        }

        [Fact]
        public void GetsTotalComponentsCountWhenEmpty()
        {
            _entity.totalComponents.Should().Be(CID.TotalComponents);
        }

        [Fact]
        public void GetsEmptyArrayOfComponentsWhenEmpty()
        {
            _entity.GetComponents().Should().BeEmpty();
        }

        [Fact]
        public void GetsEmptyArrayOfComponentIndexesWhenEmpty()
        {
            _entity.GetComponentIndices().Should().BeEmpty();
        }

        [Fact]
        public void DoesNotHaveComponentWhenEmpty()
        {
            _entity.HasComponentA().Should().BeFalse();
        }

        [Fact]
        public void DoesNotHaveComponentsWhenEmpty()
        {
            _entity.HasComponents(_indicesA).Should().BeFalse();
        }

        [Fact]
        public void DoesNotHaveAnyComponentsWhenEmpty()
        {
            _entity.HasAnyComponent(_indicesA).Should().BeFalse();
        }

        [Fact]
        public void AddsComponent()
        {
            _entity.AddComponentA();
            AssertHasComponentA(_entity);
        }

        [Fact]
        public void ThrowsWhenRemovingComponentThatDoesNotExist()
        {
            FluentActions.Invoking(() => _entity.RemoveComponentA())
                .Should().Throw<EntityDoesNotHaveComponentException>();
        }

        [Fact]
        public void ReplacingNonExistingComponentAddsComponent()
        {
            _entity.ReplaceComponentA(Component.A);
            AssertHasComponentA(_entity);
        }

        [Fact]
        public void ThrowsWhenComponentNamesLengthIsNotEqualToTotalComponents()
        {
            _entity.AddComponentA();
            FluentActions.Invoking(() =>
                    _entity.AddComponentA())
                .Should().Throw<EntityAlreadyHasComponentException>();
        }

        [Fact]
        public void RemovesComponent()
        {
            _entity.AddComponentA();
            _entity.RemoveComponentA();
            AssertHasNotComponentA(_entity);
        }

        [Fact]
        public void ReplacesExistingComponent()
        {
            _entity.AddComponentA();
            var newComponentA = new ComponentA();
            _entity.ReplaceComponentA(newComponentA);
            AssertHasComponentA(_entity, newComponentA);
        }

        [Fact]
        public void DoesNotHaveAllComponentsWhenNotAllComponentsWereAdded()
        {
            _entity.AddComponentA();
            _entity.HasComponents(_indicesAB).Should().BeFalse();
        }

        [Fact]
        public void HasAnyComponentsWhenAnyComponentWasAdded()
        {
            _entity.AddComponentA();
            _entity.HasAnyComponent(_indicesAB).Should().BeTrue();
        }

        [Fact]
        public void GetsAllComponents()
        {
            _entity.AddComponentA();
            _entity.AddComponentB();
            var components = _entity.GetComponents();
            components.Length.Should().Be(2);
            components.Should().Contain(Component.A);
            components.Should().Contain(Component.B);
        }

        [Fact]
        public void GetsAllComponentIndexes()
        {
            _entity.AddComponentA();
            _entity.AddComponentB();
            var componentIndices = _entity.GetComponentIndices();
            componentIndices.Length.Should().Be(2);
            componentIndices.Should().Contain(CID.ComponentA);
            componentIndices.Should().Contain(CID.ComponentB);
        }

        [Fact]
        public void HasOtherComponent()
        {
            _entity.AddComponentA();
            _entity.AddComponentB();
            _entity.HasComponentB().Should().BeTrue();
        }

        [Fact]
        public void HasComponentsWhenAllComponentsWereAdded()
        {
            _entity.AddComponentA();
            _entity.AddComponentB();
            _entity.HasComponents(_indicesAB).Should().BeTrue();
        }

        [Fact]
        public void RemovesAllComponents()
        {
            _entity.AddComponentA();
            _entity.AddComponentB();
            _entity.RemoveAllComponents();
            _entity.HasComponentA().Should().BeFalse();
            _entity.HasComponentB().Should().BeFalse();
            _entity.GetComponents().Should().BeEmpty();
            _entity.GetComponentIndices().Should().BeEmpty();
        }

        [Fact]
        public void ToStringDoesNotRemoveComponentSuffix()
        {
            _entity.AddComponent(0, new StandardComponent());
            _entity.Retain(this);
            _entity.ToString().Should().Be("Entity_0(StandardComponent)");
        }

        [Fact]
        public void UsesComponentToString()
        {
            _entity.AddComponent(0, new NameAgeComponent {name = "Max", age = 42});
            _entity.ToString().Should().Be("Entity_0(NameAge(Max, 42))");
        }

        [Fact]
        public void UsesFullComponentNameWithNamespaceIfToStringIsNotImplemented()
        {
            _entity.AddComponent(0, new MyNamespaceComponent());
            _entity.ToString().Should().Be("Entity_0(My.Namespace.MyNamespaceComponent)");
        }

        [Fact]
        public void GetsComponentPool()
        {
            var componentPool = _entity.GetComponentPool(CID.ComponentA);
            componentPool.Count.Should().Be(0);
        }

        [Fact]
        public void GetsSameComponentPoolInstance()
        {
            _entity.GetComponentPool(CID.ComponentA)
                .Should().BeSameAs(_entity.GetComponentPool(CID.ComponentA));
        }

        [Fact]
        public void PushesComponentToComponentPoolWhenRemoved()
        {
            _entity.AddComponentA();
            var component = _entity.GetComponentA();
            _entity.RemoveComponentA();

            var componentPool = _entity.GetComponentPool(CID.ComponentA);
            componentPool.Count.Should().Be(1);
            componentPool.Pop().Should().BeSameAs(component);
        }

        [Fact]
        public void CreatesNewComponentWhenComponentPoolIsEmpty()
        {
            var type = typeof(NameAgeComponent);
            var component = _entity.CreateComponent(1, type);
            component.GetType().Should().Be(type);

            var nameAgeComponent = ((NameAgeComponent)component);
            nameAgeComponent.name.Should().BeNull();
            nameAgeComponent.age.Should().Be(0);
        }

        [Fact]
        public void GetsPooledComponent()
        {
            var component = new NameAgeComponent();
            _entity.AddComponent(1, component);
            _entity.RemoveComponent(1);
            var newComponent = (NameAgeComponent)_entity.CreateComponent(1, typeof(NameAgeComponent));
            newComponent.Should().BeSameAs(component);
        }

        [Fact]
        public void DispatchesOnComponentAddedWhenAddingComponent()
        {
            var didDispatch = 0;
            _entity.OnComponentAdded += (entity, index, component) =>
            {
                didDispatch += 1;
                entity.Should().BeSameAs(_entity);
                index.Should().Be(CID.ComponentA);
                component.Should().BeSameAs(Component.A);
            };
            _entity.OnComponentRemoved += delegate { throw new Exception("entity.OnComponentRemoved"); };
            _entity.OnComponentReplaced += delegate { throw new Exception("entity.OnComponentReplaced"); };

            _entity.AddComponentA();
            didDispatch.Should().Be(1);
        }

        [Fact]
        public void DispatchesOnComponentRemovedWhenRemovingComponent()
        {
            var didDispatch = 0;
            _entity.AddComponentA();

            _entity.OnComponentRemoved += (entity, index, component) =>
            {
                didDispatch += 1;
                entity.Should().BeSameAs(_entity);
                index.Should().Be(CID.ComponentA);
                component.Should().BeSameAs(Component.A);
            };
            _entity.OnComponentAdded += delegate { throw new Exception("entity.OnComponentAdded"); };
            _entity.OnComponentReplaced += delegate { throw new Exception("entity.OnComponentReplaced"); };

            _entity.RemoveComponentA();
            didDispatch.Should().Be(1);
        }

        [Fact]
        public void DispatchesOnComponentRemovedBeforePushingComponentToPool()
        {
            _entity.AddComponentA();

            _entity.OnComponentRemoved += (entity, index, component) =>
            {
                var newComponent = entity.CreateComponent(index, component.GetType());
                component.Should().NotBeSameAs(newComponent);
            };

            _entity.RemoveComponentA();
        }

        [Fact]
        public void DispatchesOnComponentReplacedWhenReplacingComponent()
        {
            var didDispatch = 0;
            _entity.AddComponentA();
            var newComponentA = new ComponentA();

            _entity.OnComponentReplaced += (entity, index, previousComponent, newComponent) =>
            {
                didDispatch += 1;
                entity.Should().BeSameAs(_entity);
                index.Should().Be(CID.ComponentA);
                previousComponent.Should().BeSameAs(Component.A);
                newComponent.Should().BeSameAs(newComponentA);
            };
            _entity.OnComponentAdded += delegate { throw new Exception("OnComponentAdded"); };
            _entity.OnComponentRemoved += delegate { throw new Exception("OnComponentRemoved"); };

            _entity.ReplaceComponentA(newComponentA);
            didDispatch.Should().Be(1);
        }

        [Fact]
        public void ProvidesPreviousAndNewComponentOnComponentReplacedWhenReplacingWithDifferentComponent()
        {
            var didDispatch = 0;
            var prevComp = new ComponentA();
            var newComp = new ComponentA();

            _entity.OnComponentReplaced += (entity, _, previousComponent, newComponent) =>
            {
                didDispatch += 1;
                entity.Should().BeSameAs(_entity);
                previousComponent.Should().BeSameAs(prevComp);
                newComponent.Should().BeSameAs(newComp);
            };

            _entity.AddComponent(CID.ComponentA, prevComp);
            _entity.ReplaceComponent(CID.ComponentA, newComp);
            didDispatch.Should().Be(1);
        }

        [Fact]
        public void ProvidesPreviousAndNewComponentOnComponentReplacedWhenReplacingWithSameComponent()
        {
            var didDispatch = 0;
            _entity.OnComponentReplaced += (entity, _, previousComponent, newComponent) =>
            {
                didDispatch += 1;
                entity.Should().BeSameAs(_entity);
                previousComponent.Should().BeSameAs(Component.A);
                newComponent.Should().BeSameAs(Component.A);
            };

            _entity.AddComponentA();
            _entity.ReplaceComponentA(Component.A);
            didDispatch.Should().Be(1);
        }

        [Fact]
        public void DoesNotDispatchAnythingWhenReplacingNonExistingComponentWithNull()
        {
            _entity.OnComponentAdded += delegate { throw new Exception("entity.OnComponentAdded"); };
            _entity.OnComponentReplaced += delegate { throw new Exception("entity.OnComponentReplaced"); };
            _entity.OnComponentRemoved += delegate { throw new Exception("entity.OnComponentRemoved"); };
            _entity.ReplaceComponentA(null);
        }

        [Fact]
        public void DispatchesOnComponentAddedWhenReplaceComponentWhichHasNotBeenAdded()
        {
            var didDispatch = 0;
            var newComponentA = new ComponentA();

            _entity.OnComponentAdded += (entity, index, component) =>
            {
                didDispatch += 1;
                entity.Should().BeSameAs(_entity);
                index.Should().Be(CID.ComponentA);
                component.Should().BeSameAs(newComponentA);
            };
            _entity.OnComponentReplaced += delegate { throw new Exception("entity.OnComponentReplaced"); };
            _entity.OnComponentRemoved += delegate { throw new Exception("entity.OnComponentRemoved"); };

            _entity.ReplaceComponentA(newComponentA);
            didDispatch.Should().Be(1);
        }

        [Fact]
        public void DispatchesOnComponentRemovedWhenReplacingComponentWithNull()
        {
            var didDispatch = 0;
            _entity.AddComponentA();

            _entity.OnComponentRemoved += (_, _, component) =>
            {
                didDispatch += 1;
                component.Should().BeSameAs(Component.A);
            };
            _entity.OnComponentAdded += delegate { throw new Exception("entity.OnComponentAdded"); };
            _entity.OnComponentReplaced += delegate { throw new Exception("entity.OnComponentReplaced"); };

            _entity.ReplaceComponentA(null);
            didDispatch.Should().Be(1);
        }

        [Fact]
        public void DispatchesOnComponentRemovedWhenRemovingAllComponents()
        {
            var didDispatch = 0;
            _entity.AddComponentA();
            _entity.AddComponentB();
            _entity.OnComponentRemoved += delegate { didDispatch += 1; };
            _entity.RemoveAllComponents();
            didDispatch.Should().Be(2);
        }

        [Fact]
        public void DispatchesOnDestroyWhenCallingDestroy()
        {
            var didDispatch = 0;
            _entity.OnDestroyEntity += delegate { didDispatch += 1; };
            _entity.Destroy();
            didDispatch.Should().Be(1);
        }

        [Fact]
        public void RetainsEntity()
        {
            _entity.retainCount.Should().Be(0);
            _entity.Retain(this);
            _entity.retainCount.Should().Be(1);
            (_entity.aerc as SafeAERC)?.owners.Should().Contain(this);
        }

        [Fact]
        public void ReleasesEntity()
        {
            _entity.Retain(this);
            _entity.Release(this);
            _entity.retainCount.Should().Be(0);
            (_entity.aerc as SafeAERC)?.owners.Should().NotContain(this);
        }

        [Fact]
        public void ThrowsWhenReleasingMoreThanItHasBeenRetained()
        {
            _entity.Retain(this);
            _entity.Release(this);
            FluentActions.Invoking(() => _entity.Release(this))
                .Should().Throw<EntityIsNotRetainedByOwnerException>();
        }

        [Fact]
        public void ThrowsWhenRetainingTwiceWithSameOwner()
        {
            var owner = new object();
            _entity.Retain(owner);
            FluentActions.Invoking(() => _entity.Retain(owner))
                .Should().Throw<EntityIsAlreadyRetainedByOwnerException>();
        }

        [Fact]
        public void ThrowsWhenReleasingWithUnknownOwner()
        {
            _entity.Retain(new object());
            FluentActions.Invoking(() => _entity.Release(new object()))
                .Should().Throw<EntityIsNotRetainedByOwnerException>();
        }

        [Fact]
        public void ThrowsWhenReleasingWithOwnerWhichDoesNotRetainEntityAnymore()
        {
            var owner1 = new object();
            var owner2 = new object();
            _entity.Retain(owner1);
            _entity.Retain(owner2);
            _entity.Release(owner2);
            FluentActions.Invoking(() => _entity.Release(owner2))
                .Should().Throw<EntityIsNotRetainedByOwnerException>();
        }

        [Fact]
        public void DoesNotDispatchOnEntityReleasedWhenRetaining()
        {
            _entity.OnEntityReleased += delegate { throw new Exception("entity.OnEntityReleased"); };
            _entity.Retain(this);
        }

        [Fact]
        public void DispatchesOnEntityReleasedWhenRetainAndRelease()
        {
            var didDispatch = 0;
            _entity.OnEntityReleased += entity =>
            {
                didDispatch += 1;
                entity.Should().BeSameAs(_entity);
            };

            _entity.Retain(this);
            _entity.Release(this);
            didDispatch.Should().Be(1);
        }

        [Fact]
        public void CachesComponentsAndIndexes()
        {
            _entity.AddComponentA();
            _entity.GetComponents().Should().BeSameAs(_entity.GetComponents());
            _entity.GetComponentIndices().Should().BeSameAs(_entity.GetComponentIndices());
        }

        [Fact]
        public void UpdatesCacheWhenNewComponentIsAdded()
        {
            _entity.AddComponentA();
            var components = _entity.GetComponents();
            var indexes = _entity.GetComponentIndices();
            _entity.AddComponentB();
            _entity.GetComponents().Should().NotBeSameAs(components);
            _entity.GetComponentIndices().Should().NotBeSameAs(indexes);
        }

        [Fact]
        public void UpdatesCacheWhenComponentIsRemoved()
        {
            _entity.AddComponentA();
            var components = _entity.GetComponents();
            var indexes = _entity.GetComponentIndices();
            _entity.RemoveComponentA();
            _entity.GetComponents().Should().NotBeSameAs(components);
            _entity.GetComponentIndices().Should().NotBeSameAs(indexes);
        }

        [Fact]
        public void UpdatesComponentsCacheButNotIndexesCacheWhenComponentIsReplaced()
        {
            _entity.AddComponentA();
            var components = _entity.GetComponents();
            var indexes = _entity.GetComponentIndices();
            _entity.ReplaceComponentA(new ComponentA());
            _entity.GetComponents().Should().NotBeSameAs(components);
            _entity.GetComponentIndices().Should().BeSameAs(indexes);
        }

        [Fact]
        public void UpdatesIndexesCacheWhenAddingNewComponentWithReplaceComponent()
        {
            _entity.AddComponentA();
            var indexes = _entity.GetComponentIndices();
            _entity.ReplaceComponentC(Component.C);
            _entity.GetComponentIndices().Should().NotBeSameAs(indexes);
        }

        [Fact]
        public void DoesNotUpdateCacheWhenComponentIsReplacedWithSameComponent()
        {
            _entity.AddComponentA();
            var components = _entity.GetComponents();
            var indexes = _entity.GetComponentIndices();
            _entity.ReplaceComponentA(Component.A);
            _entity.GetComponents().Should().BeSameAs(components);
            _entity.GetComponentIndices().Should().BeSameAs(indexes);
        }

        [Fact]
        public void UpdatesCacheWhenAllComponentsAreRemoved()
        {
            _entity.AddComponentA();
            var components = _entity.GetComponents();
            var indexes = _entity.GetComponentIndices();
            _entity.RemoveAllComponents();
            _entity.GetComponents().Should().NotBeSameAs(components);
            _entity.GetComponentIndices().Should().NotBeSameAs(indexes);
        }

        [Fact]
        public void CachesEntityDescription()
        {
            _entity.AddComponentA();
            _entity.ToString().Should().BeSameAs(_entity.ToString());
        }

        [Fact]
        public void UIpdatesCacheWhenNewComponentWasAdded()
        {
            _entity.AddComponentA();
            var cache = _entity.ToString();
            _entity.AddComponentB();
            _entity.ToString().Should().NotBeSameAs(cache);
        }

        [Fact]
        public void UpdatesCacheWhenComponentWasRemoved()
        {
            _entity.AddComponentA();
            var cache = _entity.ToString();
            _entity.RemoveComponentA();
            _entity.ToString().Should().NotBeSameAs(cache);
        }

        [Fact]
        public void DoesNotUpdateCacheWhenComponentWasReplaced()
        {
            _entity.AddComponentA();
            var cache = _entity.ToString();
            _entity.ReplaceComponentA(new ComponentA());
            _entity.ToString().Should().BeSameAs(cache);
        }

        [Fact]
        public void UpdatesCacheWhenAllComponentsWereRemoved()
        {
            _entity.AddComponentA();
            var cache = _entity.ToString();
            _entity.RemoveAllComponents();
            _entity.ToString().Should().NotBeSameAs(cache);
        }

        [Fact]
        public void DoesNotUpdateCacheWhenEntityGetsRetained()
        {
            _entity.AddComponentA();
            var cache = _entity.ToString();
            _entity.Retain(this);
            _entity.ToString().Should().BeSameAs(cache);
        }

        [Fact]
        public void DoesNotUpdateCacheWhenEntityGetsReleased()
        {
            _entity.Retain(this);
            _entity.Retain(new object());
            var cache = _entity.ToString();
            _entity.Release(this);
            _entity.ToString().Should().BeSameAs(cache);
        }

        [Fact]
        public void ReleasedEntityDoesNotHaveUpdatedCache()
        {
            _entity.Retain(this);
            var cache = _entity.ToString();
            _entity.OnEntityReleased += _ => { _entity.ToString().Should().BeSameAs(cache); };
            _entity.Release(this);
        }

        [Fact]
        public void UpdatesCacheWhenRemoveAllComponentsIsCalledEvenIfEntityHasNoComponents()
        {
            var cache = _entity.ToString();
            _entity.RemoveAllComponents();
            _entity.ToString().Should().NotBeSameAs(cache);
        }

        void AssertHasComponentA(TestEntity e, IComponent componentA = null)
        {
            componentA ??= Component.A;

            e.GetComponentA().Should().BeSameAs(componentA);

            var components = e.GetComponents();
            components.Length.Should().Be(1);
            components.Should().Contain(componentA);

            var indices = e.GetComponentIndices();
            indices.Length.Should().Be(1);
            indices.Should().Contain(CID.ComponentA);

            e.HasComponentA().Should().BeTrue();
            e.HasComponents(_indicesA).Should().BeTrue();
            e.HasAnyComponent(_indicesA).Should().BeTrue();
        }

        void AssertHasNotComponentA(TestEntity e)
        {
            var components = e.GetComponents();
            components.Length.Should().Be(0);

            var indices = e.GetComponentIndices();
            indices.Length.Should().Be(0);

            e.HasComponentA().Should().BeFalse();
            e.HasComponents(_indicesA).Should().BeFalse();
            e.HasAnyComponent(_indicesA).Should().BeFalse();
        }
    }
}
