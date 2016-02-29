using NSpec;
using Entitas;
using System;

class describe_Blueprints : nspec {

    void when_creating_component_blueprints() {

        it["creates componentBlueprint"] = () => {
            var fields = new SerializableField[0];
            var type = typeof(ComponentA).FullName;
            var componentBlueprint = new ComponentBlueprint(0, type, fields);
            componentBlueprint.index.should_be(0);
            componentBlueprint.fullTypeName.should_be(type);
            componentBlueprint.fields.should_be_same(fields);
        };

        it["creates a component of type"] = () => {
            var componentBlueprint = new ComponentBlueprint(0, typeof(ComponentA).FullName, null);
            var component = componentBlueprint.CreateComponent();
            componentBlueprint.type.should_be(typeof(ComponentA));
            component.GetType().should_be(typeof(ComponentA));
        };

        it["creates a component with fields values set"] = () => {
            var fields = new [] {
                new SerializableField { fieldName = "name", value = "Max" },
                new SerializableField { fieldName = "age", value = 42 }
            };
            var componentBlueprint = new ComponentBlueprint(0, typeof(NameAgeComponent).FullName, fields);
            var component = (NameAgeComponent)componentBlueprint.CreateComponent();
            component.GetType().should_be(typeof(NameAgeComponent));

            component.name.should_be("Max");
            component.age.should_be(42);
        };

        it["throws when unknown type"] = expect<ComponentBlueprintException>(() => {
            var componentBlueprint = new ComponentBlueprint(0, "unknown", null);
            componentBlueprint.CreateComponent();
        });

        it["throws when type is doesn't implement IComponent"] = expect<ComponentBlueprintException>(() => {
            var componentBlueprint = new ComponentBlueprint(0, typeof(object).FullName, null);
            componentBlueprint.CreateComponent();
        });

        it["throws when invlaid field name"] = expect<ComponentBlueprintException>(() => {
            var fields = new [] {
                new SerializableField { fieldName = "nameXXX", value = "Max" },
                new SerializableField { fieldName = "age", value = 42 }
            };

            var componentBlueprint = new ComponentBlueprint(0, typeof(NameAgeComponent).FullName, fields);
            componentBlueprint.CreateComponent();
        });
    }

    void when_creating_blueprints() {

        it["creates Blueprint with name"] = () => {
            const string name = "My Blueprint";
            new Blueprint(name).name.should_be(name);
        };

        it["creates Blueprint with ComponentBlueprint"] = () => {
            var components = new ComponentBlueprint[0];
            var blueprint = new Blueprint(string.Empty, components);
            blueprint.components.should_be_same(components);
        };

        it["creates an entity with one component"] = () => {
            var components = new [] {
                new ComponentBlueprint(CID.ComponentA, typeof(ComponentA).FullName, null)
            };

            var blueprint = new Blueprint(string.Empty, components);

            var pool = new Pool(CID.NumComponents);
            var e = pool.CreateEntity(blueprint);

            e.GetComponents().Length.should_be(1);
            e.HasComponent(CID.ComponentA).should_be_true();
        };
    }
}

