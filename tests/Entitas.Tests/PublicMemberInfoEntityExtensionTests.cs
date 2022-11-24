using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class PublicMemberInfoEntityExtensionTests
    {
        readonly Test1Entity _entity;
        readonly Test1Entity _target;
        readonly NameAgeComponent _nameAge;

        public PublicMemberInfoEntityExtensionTests()
        {
            var context = new MyTest1Context();
            _entity = context.CreateEntity();
            _target = context.CreateEntity();
            _nameAge = new NameAgeComponent {Name = "Max", Age = 42};
        }

        [Fact]
        public void DoesNotChangeEntityIfOriginalDoesNotHaveAnyComponents()
        {
            _entity.CopyTo(_target);
            _entity.CreationIndex.Should().Be(0);
            _target.CreationIndex.Should().Be(1);
            _target.GetComponents().Length.Should().Be(0);
        }

        [Fact]
        public void AddsCopiesOfAllComponentsToTargetEntity()
        {
            _entity.AddComponentA();
            _entity.AddComponent(CID.ComponentB, _nameAge);
            _entity.CopyTo(_target);

            _target.GetComponents().Length.Should().Be(2);
            _target.HasComponentA().Should().BeTrue();
            _target.HasComponentB().Should().BeTrue();
            _target.GetComponentA().Should().NotBeSameAs(Component.A);
            _target.GetComponent(CID.ComponentB).Should().NotBeSameAs(_nameAge);

            var clonedComponent = (NameAgeComponent)_target.GetComponent(CID.ComponentB);
            clonedComponent.Name.Should().Be(_nameAge.Name);
            clonedComponent.Age.Should().Be(_nameAge.Age);
        }

        [Fact]
        public void ThrowsWhenTargetAlreadyHasComponentAtIndex()
        {
            _entity.AddComponentA();
            _entity.AddComponent(CID.ComponentB, _nameAge);
            var component = new NameAgeComponent();
            _target.AddComponent(CID.ComponentB, component);
            FluentActions.Invoking(() => _entity.CopyTo(_target))
                .Should().Throw<EntityAlreadyHasComponentException>();
        }

        [Fact]
        public void ReplacesExistingComponentsWhenOverwriteIsSet()
        {
            _entity.AddComponentA();
            _entity.AddComponent(CID.ComponentB, _nameAge);
            var component = new NameAgeComponent();
            _target.AddComponent(CID.ComponentB, component);
            _entity.CopyTo(_target, true);

            var copy = _target.GetComponent(CID.ComponentB);
            copy.Should().NotBeSameAs(_nameAge);
            copy.Should().NotBeSameAs(component);
            ((NameAgeComponent)copy).Name.Should().Be(_nameAge.Name);
            ((NameAgeComponent)copy).Age.Should().Be(_nameAge.Age);
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
