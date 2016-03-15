using Entitas;
using Entitas.Serialization.Blueprints;
using NSpec;

class describe_Blueprints : nspec {

    void when_creating() {

        context["ComponentBlueprint"] = () => {
            
            it["creates a component blueprint from a component without members"] = () => {
                var component = new ComponentA();

                const int index = 42;

                var componentBlueprint = new ComponentBlueprint(index, component);
                componentBlueprint.index.should_be(index);
                componentBlueprint.fullTypeName.should_be(component.GetType().FullName);
                componentBlueprint.members.Length.should_be(0);
            };

            it["creates a component blueprint from a component with members"] = () => {
                var component = new NameAgeComponent();
                component.name = "Max";
                component.age = 42;

                const int index = 24;

                var componentBlueprint = new ComponentBlueprint(index, component);
                componentBlueprint.index.should_be(index);
                componentBlueprint.fullTypeName.should_be(component.GetType().FullName);
                componentBlueprint.members.Length.should_be(2);

                componentBlueprint.members[0].fieldName.should_be("name");
                componentBlueprint.members[0].value.should_be(component.name);

                componentBlueprint.members[1].fieldName.should_be("age");
                componentBlueprint.members[1].value.should_be(component.age);
            };
        };

        context["Blueprint"] = () => {

            it["creates a blueprint from an entity"] = () => {
                var pool = new Pool(CID.NumComponents);
                var e = pool.CreateEntity();
                e.AddComponentA();

                var component = new NameAgeComponent();
                component.name = "Max";
                component.age = 42;

                e.AddComponent(CID.ComponentB, component);

                var blueprint = new Blueprint("Hero", e);
                blueprint.name.should_be("Hero");
                blueprint.components.Length.should_be(2);

                blueprint.components[0].index.should_be(CID.ComponentA);
                blueprint.components[0].fullTypeName.should_be(Component.A.GetType().FullName);

                blueprint.components[1].index.should_be(CID.ComponentB);
                blueprint.components[1].fullTypeName.should_be(component.GetType().FullName);
            };
        };
    }
}

//Hero:
//  PositionComponent:
//  index: 42
//    fields:
//      x: 1
//      y: 2
//      z: 3
//  ResourceComponent:
//    index: 24
//    fields:
//      name: "Gem0"


//{
//    "Hero": {
//        "ResourceComponent": {
//            "index": 24,
//            "members": {
//                "name": "Hero"
//            }
//        },
//        "PositionComponent": {
//            "index": 42,
//            "members": {
//                "y": 2,
//                "x": 1,
//                "z": 3
//            }
//        }
//    }
//}
