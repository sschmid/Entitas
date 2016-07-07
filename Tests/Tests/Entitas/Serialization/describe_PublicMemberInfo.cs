using Entitas.Serialization;
using NSpec;

class describe_PublicMemberInfo : nspec {

    void when_creating() {

        context["when getting public member infos"] = () => {            

            it["creates empty info when component has no fields or properties"] = () => {
                var infos = typeof(ComponentA).GetPublicMemberInfos();
                infos.should_be_empty();
            };

            it["creates member infos for public fields"] = () => {
                var infos = typeof(ComponentWithFields).GetPublicMemberInfos();
                infos.Count.should_be(1);
                var mi = infos[0];
                mi.type.should_be(typeof(string));
                mi.name.should_be("publicField");
            };

            it["creates member infos for public properties (read & write)"] = () => {
                var infos = typeof(ComponentWithProperties).GetPublicMemberInfos();
                infos.Count.should_be(1);
                var mi = infos[0];
                mi.type.should_be(typeof(string));
                mi.name.should_be("publicProperty");
            };

            it["creates member infos for fields and properties"] = () => {
                var infos = typeof(ComponentWithFieldsAndProperties).GetPublicMemberInfos();
                infos.Count.should_be(2);
                var mi1 = infos[0];
                var mi2 = infos[1];

                mi1.type.should_be(typeof(string));
                mi1.name.should_be("publicField");

                mi2.type.should_be(typeof(string));
                mi2.name.should_be("publicProperty");
            };

            it["gets values for fields and properties"] = () => {
                var component = new ComponentWithFieldsAndProperties();
                component.publicField = "publicFieldValue";
                component.publicProperty = "publicPropertyValue";

                var infos = component.GetType().GetPublicMemberInfos();
                var mi1 = infos[0];
                var mi2 = infos[1];

                mi1.GetValue(component).should_be("publicFieldValue");
                mi2.GetValue(component).should_be("publicPropertyValue");
            };

            it["sets values for fields and properties"] = () => {
                var component = new ComponentWithFieldsAndProperties();

                var infos = component.GetType().GetPublicMemberInfos();
                var mi1 = infos[0];
                var mi2 = infos[1];

                mi1.SetValue(component, "publicFieldValue");
                mi2.SetValue(component, "publicPropertyValue");

                component.publicField.should_be("publicFieldValue");
                component.publicProperty.should_be("publicPropertyValue");
            };
        };

        context["when cloning object"] = () => {

            ComponentWithFieldsAndProperties component = null;
            before = () => {
                component = new ComponentWithFieldsAndProperties();
                component.publicField = "field";
                component.publicProperty = "property";
            };

            it["clones object and sets public members"] = () => {
                var clone = (ComponentWithFieldsAndProperties)component.PublicMemberClone();

                clone.should_not_be_same(component);
                clone.publicField.should_be(component.publicField);
                clone.publicProperty.should_be(component.publicProperty);
            };

            it["copies public members to other obj"] = () => {
                var newComponent = new ComponentWithFieldsAndProperties();

                component.CopyPublicMemberValues(newComponent);

                newComponent.publicField.should_be(component.publicField);
                newComponent.publicProperty.should_be(component.publicProperty);
            };
        };
    }
}

