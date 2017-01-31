using Entitas;
using NSpec;

class describe_ComponentStringExtension : nspec {

    const string COMPONENT_SUFFIX = "Component";

    void when_entity() {

        context["when adding ComponentSuffix"] = () => {

            it["doesn't add component suffix to string ending with ComponentSuffix"] = () => {
                const string str = "Position" + COMPONENT_SUFFIX;
                str.AddComponentSuffix().should_be_same(str);
            };

            it["add ComponentSuffix to string not ending with ComponentSuffix"] = () => {
                const string str = "Position";
                str.AddComponentSuffix().should_be("Position" + COMPONENT_SUFFIX);
            };
        };

        context["when removing ComponentSuffix"] = () => {

            it["doesn't change string when not ending with ComponentSuffix"] = () => {
                const string str = "Position";
                str.RemoveComponentSuffix().should_be_same(str);
            };

            it["removes ComponentSuffix when ending with ComponentSuffix"] = () => {
                const string str = "Position" + COMPONENT_SUFFIX;
                str.RemoveComponentSuffix().should_be("Position");
            };
        };
    }
}
