using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class PublicMemberInfoEntityExtensionTests
    {
        readonly TestEntity _entity;
        readonly TestEntity _target;
        readonly UserComponent _user;

        public PublicMemberInfoEntityExtensionTests()
        {
            var context = new TestContext(CID.TotalComponents);
            _entity = context.CreateEntity();
            _target = context.CreateEntity();
            _user = new UserComponent { Name = "Max", Age = 42 };
        }

        [Fact]
        public void DoesNotChangeEntityIfOriginalDoesNotHaveAnyComponents()
        {
            _entity.CopyTo(_target);
            _entity.Id.Should().Be(0);
            _target.Id.Should().Be(1);
            _target.GetComponents().Length.Should().Be(0);
        }

        [Fact]
        public void AddsCopiesOfAllComponentsToTargetEntity()
        {
            _entity.AddComponentA();
            _entity.AddComponent(CID.ComponentB, _user);
            _entity.CopyTo(_target);

            _target.GetComponents().Length.Should().Be(2);
            _target.HasComponentA().Should().BeTrue();
            _target.HasComponentB().Should().BeTrue();
            _target.GetComponentA().Should().NotBeSameAs(Component.A);
            _target.GetComponent(CID.ComponentB).Should().NotBeSameAs(_user);

            var clonedComponent = (UserComponent)_target.GetComponent(CID.ComponentB);
            clonedComponent.Name.Should().Be(_user.Name);
            clonedComponent.Age.Should().Be(_user.Age);
        }

        [Fact]
        public void ThrowsWhenTargetAlreadyHasComponentAtIndex()
        {
            _entity.AddComponentA();
            _entity.AddComponent(CID.ComponentB, _user);
            var component = new UserComponent();
            _target.AddComponent(CID.ComponentB, component);
            FluentActions.Invoking(() => _entity.CopyTo(_target))
                .Should().Throw<EntityAlreadyHasComponentException>();
        }

        [Fact]
        public void ReplacesExistingComponentsWhenOverwriteIsSet()
        {
            _entity.AddComponentA();
            _entity.AddComponent(CID.ComponentB, _user);
            var component = new UserComponent();
            _target.AddComponent(CID.ComponentB, component);
            _entity.CopyTo(_target, true);

            var copy = _target.GetComponent(CID.ComponentB);
            copy.Should().NotBeSameAs(_user);
            copy.Should().NotBeSameAs(component);
            ((UserComponent)copy).Name.Should().Be(_user.Name);
            ((UserComponent)copy).Age.Should().Be(_user.Age);
        }

        [Fact]
        public void OnlyAddsCopiesOfSpecifiedComponentsToTargetEntity()
        {
            _entity.AddComponentA();
            _entity.AddComponentB();
            _entity.AddComponentC();
            _entity.CopyTo(_target, false, CID.ComponentB, CID.ComponentC);

            _target.GetComponents().Length.Should().Be(2);
            _target.HasComponentB().Should().BeTrue();
            _target.HasComponentC().Should().BeTrue();
        }

        [Fact]
        public void UsesComponentPool()
        {
            _entity.AddComponentA();
            var component = new ComponentA();
            _target.GetComponentPool(CID.ComponentA).Push(component);
            _entity.CopyTo(_target);
            _target.GetComponentA().Should().BeSameAs(component);
        }
    }
}
