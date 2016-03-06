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
                infos.Length.should_be(1);
                var mi = infos[0];
                mi.fullTypeName.should_be(typeof(string).FullName);
                mi.name.should_be("publicField");
                mi.memberType.should_be(PublicMemberInfo.MemberType.Field);
            };

            it["creates member infos for public properties (read & write)"] = () => {
                var infos = typeof(ComponentWithProperties).GetPublicMemberInfos();
                infos.Length.should_be(1);
                var mi = infos[0];
                mi.fullTypeName.should_be(typeof(string).FullName);
                mi.name.should_be("publicProperty");
                mi.memberType.should_be(PublicMemberInfo.MemberType.Property);
            };

            it["creates member infos for fields and properties"] = () => {
                var infos = typeof(ComponentWithFieldsAndProperties).GetPublicMemberInfos();
                infos.Length.should_be(2);
                var mi1 = infos[0];
                var mi2 = infos[1];

                mi1.fullTypeName.should_be(typeof(string).FullName);
                mi1.name.should_be("publicField");
                mi1.memberType.should_be(PublicMemberInfo.MemberType.Field);

                mi2.fullTypeName.should_be(typeof(string).FullName);
                mi2.name.should_be("publicProperty");
                mi2.memberType.should_be(PublicMemberInfo.MemberType.Property);
            };

            it["creates member info with compilable type string"] = () => {
                var infos = typeof(ComponentWithFieldsAndProperties).GetPublicMemberInfos(true);
                var mi1 = infos[0];
                var mi2 = infos[1];

                mi1.fullTypeName.should_be(typeof(string).ToCompilableString());
                mi2.fullTypeName.should_be(typeof(string).ToCompilableString());
            };
        };

        context["when getting public member infos with value"] = () => {

            it["creates member infos for fields and properties"] = () => {
                var component = new ComponentWithFieldsAndProperties();
                component.publicField = "publicFieldValue";
                component.publicProperty = "publicPropertyValue";

                var infos = component.GetPublicMemberInfos();
                infos.Length.should_be(2);
                var mi1 = infos[0];
                var mi2 = infos[1];

                mi1.fullTypeName.should_be(typeof(string).FullName);
                mi1.name.should_be("publicField");
                mi1.memberType.should_be(PublicMemberInfo.MemberType.Field);
                mi1.value.should_be("publicFieldValue");

                mi2.fullTypeName.should_be(typeof(string).FullName);
                mi2.name.should_be("publicProperty");
                mi2.memberType.should_be(PublicMemberInfo.MemberType.Property);
                mi2.value.should_be("publicPropertyValue");
            };

            it["creates member info with compilable type string"] = () => {
                var component = new ComponentWithFieldsAndProperties();
                var infos = component.GetPublicMemberInfos(true);
                var mi1 = infos[0];
                var mi2 = infos[1];

                mi1.fullTypeName.should_be(typeof(string).ToCompilableString());
                mi2.fullTypeName.should_be(typeof(string).ToCompilableString());
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

