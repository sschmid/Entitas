using System;
using System.Collections.Generic;
using Entitas.Utils;
using NSpec;

class describe_Preferences : nspec {

    void when_creating_preferences() {

        Properties properties = null;
        Properties userProperties = null;
        Preferences preferences = null;
        context["when properties"] = () => {

            before = () => {
                const string input = "key = value" + "\n";
                properties = new Properties(input);
                preferences = new Preferences(properties);
            };

            it["sets properties"] = () => {
                preferences.properties.should_be_same(properties);
                preferences.userProperties.count.should_be(0);
            };

            it["gets value for key"] = () => {
                preferences["key"].should_be("value");
            };

            it["throws"] = expect<KeyNotFoundException>(() => {
                var x = preferences["unknown"];
            });

            it["has key"] = () => {
                preferences.HasKey("key").should_be_true();
            };

            it["sets key"] = () => {
                preferences["key2"] = "value2";
                preferences["key2"].should_be("value2");
                preferences.HasKey("key2").should_be_true();
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

            it["sets properties"] = () => {
                preferences.properties.should_be_same(properties);
                preferences.userProperties.should_be_same(userProperties);
            };

            it["has key"] = () => {
                preferences.HasKey("userName").should_be_true();
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

            it["can ToString"] = () => {
                preferences.ToString().should_be(properties.ToString() + userProperties.ToString());
            };
        };
    }
}
