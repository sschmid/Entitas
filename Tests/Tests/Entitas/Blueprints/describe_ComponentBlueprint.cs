using System.Collections.Generic;
using Entitas;
using Entitas.Serialization;
using My.Namespace;
using NSpec;

class describe_ComponentBlueprint : nspec {

    void when_componentBlueprint() {

        it["creates a ComponentBlueprint from a component without public members"] = () => {
            var component = new NamespaceComponent();
            var blueprint = new ComponentBlueprint(component);

            blueprint.name.should_be(component.GetType().FullName);
        };

        it["creates a ComponentBlueprint from a component with public members"] = () => {
            var component = new ComponentWithFieldsAndProperties();
            component.publicField = "publicFieldValue";
            component.publicProperty = "publicPropertyValue";

            var blueprint = new ComponentBlueprint(component);

            blueprint.name.should_be(component.GetType().FullName);
            blueprint.properties.Count.should_be(2);
            blueprint.properties["publicField"].should_be("publicFieldValue");
            blueprint.properties["publicProperty"].should_be("publicPropertyValue");
        };

        it["ignores null value"] = () => {
            var component = new ComponentWithFieldsAndProperties();
            var blueprint = new ComponentBlueprint(component);

            blueprint.name.should_be(component.GetType().FullName);
            blueprint.properties.Count.should_be(2);

            blueprint.properties["publicField"].should_be(string.Empty);
            blueprint.properties["publicProperty"].should_be(string.Empty);
        };
    }
}

public struct ComponentBlueprint {

    public readonly string name;
    public readonly Dictionary<string, string> properties;

    public ComponentBlueprint(IComponent component) {
        name = component.GetType().FullName;
        properties = new Dictionary<string, string>();

        var infos = component.GetPublicMemberInfos();
        for (int i = 0, infosLength = infos.Length; i < infosLength; i++) {
            var info = infos[i];
            properties.Add(info.name, info.value != null ? info.value.ToString() : string.Empty);
        }
    }
}