using NSpec;
using Entitas;

class describe_EntityExtension : nspec {

    void when_entity() {

        context["when adding ComponentSuffix"] = () => {

            it["doesn't add component suffix to string ending with ComponentSuffix"] = () => {
                const string str = "Position" + EntityExtension.COMPONENT_SUFFIX;
                str.AddComponentSuffix().should_be_same(str);
            };

            it["add ComponentSuffix to string not ending with ComponentSuffix"] = () => {
                const string str = "Position";
                str.AddComponentSuffix().should_be("Position" + EntityExtension.COMPONENT_SUFFIX);
            };
        };

        context["when removeing ComponentSuffix"] = () => {

            it["doesn't change string when not ending with ComponentSuffix"] = () => {
                const string str = "Position";
                str.RemoveComponentSuffix().should_be_same(str);
            };

            it["removes ComponentSuffix when ending with ComponentSuffix"] = () => {
                const string str = "Position" + EntityExtension.COMPONENT_SUFFIX;
                str.RemoveComponentSuffix().should_be("Position");
            };
        };
    }
}
