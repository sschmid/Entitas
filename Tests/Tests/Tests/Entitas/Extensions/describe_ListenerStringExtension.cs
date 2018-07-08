using Entitas;
using NSpec;

class describe_ListenerStringExtension : nspec {

    const string LISTENER_SUFFIX = "Listener";

    void when_listener() {

        context["when adding ListenerSuffix"] = () => {

            it["doesn't add listener suffix to string ending with ListenerSuffix"] = () => {
                const string str = "Position" + LISTENER_SUFFIX;
                str.AddListenerSuffix().should_be_same(str);
            };

            it["adds ListenerSuffix to string not ending with ListenerSuffix"] = () => {
                const string str = "Position";
                str.AddListenerSuffix().should_be("Position" + LISTENER_SUFFIX);
            };
        };

        context["when removing ListenerSuffix"] = () => {

            it["doesn't change string when not ending with ListenerSuffix"] = () => {
                const string str = "Position";
                str.RemoveListenerSuffix().should_be_same(str);
            };

            it["removes ListenerSuffix when ending with ListenerSuffix"] = () => {
                const string str = "Position" + LISTENER_SUFFIX;
                str.RemoveListenerSuffix().should_be("Position");
            };
        };
    }
}
