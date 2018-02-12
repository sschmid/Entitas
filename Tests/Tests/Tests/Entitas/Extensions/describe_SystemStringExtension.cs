using Entitas;
using NSpec;

class describe_SystemStringExtension : nspec {

    const string SYSTEM_SUFFIX = "System";

    void when_entity() {

        context["when adding SystemSuffix"] = () => {

            it["doesn't add system suffix to string ending with SystemSuffix"] = () => {
                const string str = "Position" + SYSTEM_SUFFIX;
                str.AddSystemSuffix().should_be_same(str);
            };

            it["adds SystemSuffix to string not ending with SystemSuffix"] = () => {
                const string str = "Position";
                str.AddSystemSuffix().should_be("Position" + SYSTEM_SUFFIX);
            };
        };

        context["when removing SystemSuffix"] = () => {

            it["doesn't change string when not ending with SystemSuffix"] = () => {
                const string str = "Position";
                str.RemoveSystemSuffix().should_be_same(str);
            };

            it["removes SystemSuffix when ending with SystemSuffix"] = () => {
                const string str = "Position" + SYSTEM_SUFFIX;
                str.RemoveSystemSuffix().should_be("Position");
            };
        };
    }
}
