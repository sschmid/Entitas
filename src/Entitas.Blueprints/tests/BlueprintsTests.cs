using FluentAssertions;
using Xunit;

namespace Entitas.Blueprints.Tests
{
    public class BlueprintsTests
    {
        readonly TestEntity _entity;

        public BlueprintsTests()
        {
            var context = new MyTestContext();
            _entity = context.CreateEntity();
        }

        [Fact]
        public void CreatesComponentBlueprintFromComponentWithoutMembers()
        {
            var component = new ComponentA();
            var componentBlueprint = new ComponentBlueprint(42, component);
            componentBlueprint.index.Should().Be(42);
            componentBlueprint.fullTypeName.Should().Be(component.GetType().FullName);
            componentBlueprint.members.Length.Should().Be(0);
        }

        [Fact]
        public void ThrowsWhenUnknownType()
        {
            var componentBlueprint = new ComponentBlueprint();
            componentBlueprint.fullTypeName = "UnknownType";
            FluentActions.Invoking(() => componentBlueprint.CreateComponent(null))
                .Should().Throw<ComponentBlueprintException>();
        }

        [Fact]
        public void ThrowsWhenTypeDoesNotImplementIComponent()
        {
            var componentBlueprint = new ComponentBlueprint();
            componentBlueprint.fullTypeName = "string";
            FluentActions.Invoking(() => componentBlueprint.CreateComponent(null))
                .Should().Throw<ComponentBlueprintException>();
        }

        [Fact]
        public void CreatesComponentBlueprintFromComponentWithMembers()
        {
            var component = new NameAgeComponent();
            component.name = "Max";
            component.age = 42;

            var componentBlueprint = new ComponentBlueprint(24, component);
            componentBlueprint.index.Should().Be(24);
            componentBlueprint.fullTypeName.Should().Be(component.GetType().FullName);
            componentBlueprint.members.Length.Should().Be(2);

            componentBlueprint.members[0].name.Should().Be("name");
            componentBlueprint.members[0].value.Should().Be(component.name);

            componentBlueprint.members[1].name.Should().Be("age");
            componentBlueprint.members[1].value.Should().Be(component.age);
        }

        [Fact]
        public void CreatesComponentAndSetsMembersValues()
        {
            var componentBlueprint = new ComponentBlueprint();
            componentBlueprint.fullTypeName = typeof(ComponentWithFieldsAndProperties).FullName;
            componentBlueprint.index = CID.ComponentB;
            componentBlueprint.members = new[]
            {
                new SerializableMember("publicField", "publicFieldValue"),
                new SerializableMember("publicProperty", "publicPropertyValue")
            };

            var component = (ComponentWithFieldsAndProperties)componentBlueprint.CreateComponent(_entity);
            component.publicField.Should().Be("publicFieldValue");
            component.publicProperty.Should().Be("publicPropertyValue");
        }

        [Fact]
        public void IgnoresInvalidMemberName()
        {
            var componentBlueprint = new ComponentBlueprint();
            componentBlueprint.index = 0;
            componentBlueprint.fullTypeName = typeof(NameAgeComponent).FullName;
            componentBlueprint.members = new[]
            {
                new SerializableMember("xxx", "publicFieldValue"),
                new SerializableMember("publicProperty", "publicPropertyValue")
            };
            componentBlueprint.CreateComponent(_entity);
        }

        [Fact]
        public void CreatesEmptyBlueprintFromNull()
        {
            var blueprint = new Blueprint("My Context", "Hero", null);
            blueprint.contextIdentifier.Should().Be("My Context");
            blueprint.name.Should().Be("Hero");
            blueprint.components.Length.Should().Be(0);
        }

        [Fact]
        public void CreatesBlueprintFromEntity()
        {
            _entity.AddComponentA();

            var component = new NameAgeComponent();
            component.name = "Max";
            component.age = 42;

            _entity.AddComponent(CID.ComponentB, component);

            var blueprint = new Blueprint("My Context", "Hero", _entity);
            blueprint.contextIdentifier.Should().Be("My Context");
            blueprint.name.Should().Be("Hero");
            blueprint.components.Length.Should().Be(2);

            blueprint.components[0].index.Should().Be(CID.ComponentA);
            blueprint.components[0].fullTypeName.Should().Be(Component.A.GetType().FullName);

            blueprint.components[1].index.Should().Be(CID.ComponentB);
            blueprint.components[1].fullTypeName.Should().Be(component.GetType().FullName);
        }

        [Fact]
        public void AppliesBlueprintToEntity()
        {
            var blueprint = CreateBlueprint();
            _entity.ApplyBlueprint(blueprint);
            _entity.GetComponents().Length.Should().Be(2);
            _entity.GetComponent(CID.ComponentA).GetType().Should().Be(typeof(ComponentA));
            var nameAgeComponent = (NameAgeComponent)_entity.GetComponent(CID.ComponentB);
            nameAgeComponent.GetType().Should().Be(typeof(NameAgeComponent));
            nameAgeComponent.name.Should().Be("Max");
            nameAgeComponent.age.Should().Be(42);
        }

        [Fact]
        public void ThrowsWhenEntityAlreadyHasComponentWhichShouldBeAddedFromBlueprint()
        {
            var blueprint = CreateBlueprint();
            _entity.AddComponentA();
            FluentActions.Invoking(() => _entity.ApplyBlueprint(blueprint))
                .Should().Throw<EntityAlreadyHasComponentException>();
        }

        [Fact]
        public void CanOverwriteExistingComponents()
        {
            var blueprint = CreateBlueprint();
            var nameAgeComponent = new NameAgeComponent();
            nameAgeComponent.name = "Jack";
            nameAgeComponent.age = 24;
            _entity.AddComponent(CID.ComponentB, nameAgeComponent);
            _entity.ApplyBlueprint(blueprint, true);
        }

        [Fact]
        public void UsesComponentFromComponentPool()
        {
            var component = new ComponentBlueprint();
            component.index = CID.ComponentA;
            component.fullTypeName = typeof(ComponentA).FullName;
            component.members = new SerializableMember[0];

            var blueprint = new Blueprint();
            blueprint.name = "Hero";
            blueprint.components = new[] {component};

            var componentA = _entity.CreateComponent<ComponentA>(CID.ComponentA);
            _entity.AddComponent(CID.ComponentA, componentA);
            _entity.RemoveComponentA();
            _entity.ApplyBlueprint(blueprint);
            _entity.GetComponentA().Should().BeSameAs(componentA);
        }

        Blueprint CreateBlueprint()
        {
            var component1 = new ComponentBlueprint();
            component1.index = CID.ComponentA;
            component1.fullTypeName = typeof(ComponentA).FullName;
            component1.members = new SerializableMember[0];

            var component2 = new ComponentBlueprint();
            component2.index = CID.ComponentB;
            component2.fullTypeName = typeof(NameAgeComponent).FullName;
            component2.members = new[]
            {
                new SerializableMember("name", "Max"),
                new SerializableMember("age", 42)
            };

            var blueprint = new Blueprint();
            blueprint.name = "Hero";
            blueprint.components = new[] {component1, component2};
            return blueprint;
        }
    }
}
