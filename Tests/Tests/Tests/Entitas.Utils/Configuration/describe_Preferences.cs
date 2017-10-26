using Entitas.Utils;
using NSpec;

class describe_Preferences : nspec {

    void when_creating_preferences() {

        context["when empty"] = () => {
            it["is empty"] = () => new Preferences().ToString().should_be(string.Empty);
        };

        Properties properties = null;
        Properties userProperties = null;
        Preferences preferences = null;

        context["when properties"] = () => {

            before = () => {
                const string input = "key = value" + "\n";
                properties = new Properties(input);
                preferences = new Preferences(properties);
            };

            it["can ToString"] = () => {
                preferences.ToString().should_be(properties.ToString());
            };
        };

        context["when user properties"] = () => {

            before = () => {
                const string input = "key = ${userName}" + "\n";
                const string userInput = "userName = Max" + "\n";
                properties = new Properties(input);
                userProperties = new Properties(userInput);
                preferences = new Preferences(properties, userProperties);
            };

            it["can ToString"] = () => {
                preferences.ToString().should_be(properties + "\n" + userProperties);
            };

            it["resolves placeholder from user properties"] = () => {
                preferences["key"].should_be("Max");
            };

            it["doesn't overwrite value when not different"] = () => {
                preferences["key"] = "Max";
                preferences.properties.ToString().Contains("Max").should_be_false();
            };

            it["overwrites value when different"] = () => {
                preferences["key"] = "Jack";
                preferences.properties.ToString().Contains("Jack").should_be_true();
            };

            it["user properties overwrite default properties"] = () => {
                const string newUserInput = "key = Jack!" + "\n";
                userProperties = new Properties(newUserInput);
                preferences = new Preferences(properties, userProperties);
                preferences["key"].should_be("Jack!");
            };
        };
    }
}
