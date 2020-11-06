using Entitas;
using Entitas.Blueprints;
using NSpec;
using Shouldly;

class describe_Blueprints : nspec {

    void when_creating() {

        IContext<TestEntity> ctx = null;
        TestEntity entity = null;

        before = () => {
            ctx = new MyTestContext();
            entity = ctx.CreateEntity();
        };

        context["ComponentBlueprint"] = () => {

            it["creates a component blueprint from a component without members"] = () => {
                var component = new ComponentA();

                const int index = 42;

                var componentBlueprint = new ComponentBlueprint(index, component);
                componentBlueprint.index.ShouldBe(index);
                componentBlueprint.fullTypeName.ShouldBe(component.GetType().FullName);
                componentBlueprint.members.Length.ShouldBe(0);
            };

            it["throws when unknown type"] = expect<ComponentBlueprintException>(() => {
                var componentBlueprint = new ComponentBlueprint();
                componentBlueprint.fullTypeName = "UnknownType";
                componentBlueprint.CreateComponent(null);
            });

            it["throws when type doesn't implement IComponent"] = expect<ComponentBlueprintException>(() => {
                var componentBlueprint = new ComponentBlueprint();
                componentBlueprint.fullTypeName = "string";
                componentBlueprint.CreateComponent(null);
            });

            it["creates a component blueprint from a component with members"] = () => {
                var component = new NameAgeComponent();
                component.name = "Max";
                component.age = 42;

                const int index = 24;

                var componentBlueprint = new ComponentBlueprint(index, component);
                componentBlueprint.index.ShouldBe(index);
                componentBlueprint.fullTypeName.ShouldBe(component.GetType().FullName);
                componentBlueprint.members.Length.ShouldBe(2);

                componentBlueprint.members[0].name.ShouldBe("name");
                componentBlueprint.members[0].value.ShouldBe(component.name);

                componentBlueprint.members[1].name.ShouldBe("age");
                componentBlueprint.members[1].value.ShouldBe(component.age);
            };

            it["creates a component and sets members values"] = () => {
                var componentBlueprint = new ComponentBlueprint();
                componentBlueprint.fullTypeName = typeof(ComponentWithFieldsAndProperties).FullName;
                componentBlueprint.index = CID.ComponentB;
                componentBlueprint.members = new [] {
                    new SerializableMember("publicField", "publicFieldValue"),
                    new SerializableMember("publicProperty", "publicPropertyValue")
                };

                var component = (ComponentWithFieldsAndProperties)componentBlueprint.CreateComponent(entity);
                component.publicField.ShouldBe("publicFieldValue");
                component.publicProperty.ShouldBe("publicPropertyValue");
            };

            it["ignores invalid member name"] = () => {
                var componentBlueprint = new ComponentBlueprint();
                componentBlueprint.index = 0;
                componentBlueprint.fullTypeName = typeof(NameAgeComponent).FullName;
                componentBlueprint.members = new [] {
                    new SerializableMember("xxx", "publicFieldValue"),
                    new SerializableMember("publicProperty", "publicPropertyValue")
                };
                componentBlueprint.CreateComponent(entity);
            };
        };

        context["Blueprint"] = () => {

            it["creates an empty blueprint from a null entity"] = () => {
                var blueprint = new Blueprint("My Context", "Hero", null);
                blueprint.contextIdentifier.ShouldBe("My Context");
                blueprint.name.ShouldBe("Hero");
                blueprint.components.Length.ShouldBe(0);
            };

            it["creates a blueprint from an entity"] = () => {
                entity.AddComponentA();

                var component = new NameAgeComponent();
                component.name = "Max";
                component.age = 42;

                entity.AddComponent(CID.ComponentB, component);

                var blueprint = new Blueprint("My Context", "Hero", entity);
                blueprint.contextIdentifier.ShouldBe("My Context");
                blueprint.name.ShouldBe("Hero");
                blueprint.components.Length.ShouldBe(2);

                blueprint.components[0].index.ShouldBe(CID.ComponentA);
                blueprint.components[0].fullTypeName.ShouldBe(Component.A.GetType().FullName);

                blueprint.components[1].index.ShouldBe(CID.ComponentB);
                blueprint.components[1].fullTypeName.ShouldBe(component.GetType().FullName);
            };

            context["when applying blueprint"] = () => {

                Blueprint blueprint = null;

                before = () => {
                    var component1 = new ComponentBlueprint();
                    component1.index = CID.ComponentA;
                    component1.fullTypeName = typeof(ComponentA).FullName;
                    component1.members = new SerializableMember[0];

                    var component2 = new ComponentBlueprint();
                    component2.index = CID.ComponentB;
                    component2.fullTypeName = typeof(NameAgeComponent).FullName;
                    component2.members = new [] {
                        new SerializableMember("name", "Max"),
                        new SerializableMember("age", 42)
                    };

                    blueprint = new Blueprint();
                    blueprint.name = "Hero";
                    blueprint.components = new [] { component1, component2 };
                };

                it["applies blueprint to entity"] = () => {
                    entity.ApplyBlueprint(blueprint);
                    entity.GetComponents().Length.ShouldBe(2);

                    entity.GetComponent(CID.ComponentA).GetType().ShouldBe(typeof(ComponentA));

                    var nameAgeComponent = (NameAgeComponent)entity.GetComponent(CID.ComponentB);
                    nameAgeComponent.GetType().ShouldBe(typeof(NameAgeComponent));
                    nameAgeComponent.name.ShouldBe("Max");
                    nameAgeComponent.age.ShouldBe(42);
                };

                it["throws when entity already has a component which should be added from blueprint"] = expect<EntityAlreadyHasComponentException>(() => {
                    entity.AddComponentA();
                    entity.ApplyBlueprint(blueprint);
                });

                it["can overwrite existing components"] = () => {
                    var nameAgeComponent = new NameAgeComponent();
                    nameAgeComponent.name = "Jack";
                    nameAgeComponent.age = 24;
                    entity.AddComponent(CID.ComponentB, nameAgeComponent);

                    entity.ApplyBlueprint(blueprint, true);
                };

                it["uses component from componentPool"] = () => {
                    var component = new ComponentBlueprint();
                    component.index = CID.ComponentA;
                    component.fullTypeName = typeof(ComponentA).FullName;
                    component.members = new SerializableMember[0];

                    blueprint = new Blueprint();
                    blueprint.name = "Hero";
                    blueprint.components = new [] { component };

                    var componentA = entity.CreateComponent<ComponentA>(CID.ComponentA);
                    entity.AddComponent(CID.ComponentA, componentA);
                    entity.RemoveComponentA();

                    entity.ApplyBlueprint(blueprint);

                    entity.GetComponentA().ShouldBeSameAs(componentA);
                };
            };
        };
    }
}
