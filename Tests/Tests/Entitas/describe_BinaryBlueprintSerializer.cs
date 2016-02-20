using NSpec;
using Entitas;
using System.IO;

class describe_BinaryBlueprintSerializer : nspec {
    void when_creating_serializable_component_blueprints() {

        it["serializes and deserialized a component blueprint"] = () => {
            var component1 = new ComponentBlueprint(42, typeof(NameAgeComponent).FullName, new [] {
                new SerializableField { fieldName = "name", value = "Max" },
                new SerializableField { fieldName = "age", value = 42 }
            });

            var component2 = new ComponentBlueprint(24, typeof(NameAgeComponent).FullName, new [] {
                new SerializableField { fieldName = "name", value = "Tom" },
                new SerializableField { fieldName = "age", value = 24 }
            });

            var components = new[] { component1, component2 };
            var blueprint = new Blueprint("MyBlueprint", components);

            var serializer = new BinaryBlueprintSerializer();

            using (var stream = new MemoryStream()) {
                serializer.Serialize(blueprint, stream);
                stream.Seek(0, SeekOrigin.Begin);

                var bp = serializer.Deserialize(stream);
                bp.name.should_be("MyBlueprint");
                bp.components.Length.should_be(components.Length);
            
                var c1 = bp.components[0];
                c1.index.should_be(component1.index);
                c1.fullTypeName.should_be(component1.fullTypeName);
                c1.fields.Length.should_be(component1.fields.Length);
            
                c1.type.should_be(typeof(NameAgeComponent));
            
                var nameAge1 = (NameAgeComponent)c1.CreateComponent();
                nameAge1.name.should_be("Max");
                nameAge1.age.should_be(42);

                var c2 = bp.components[1];
                c2.index.should_be(component2.index);
                c2.fullTypeName.should_be(component2.fullTypeName);
                c2.fields.Length.should_be(component2.fields.Length);
                
                var nameAge2 = (NameAgeComponent)c2.CreateComponent();
                nameAge2.name.should_be("Tom");
                nameAge2.age.should_be(24);
            }
        };
    }
}

