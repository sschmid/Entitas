using System.Collections.Generic;
using Entitas.Utils;
using NSpec;

class describe_AbstractConfigurableConfig : nspec {

    void when_config() {

        it["adds default properties when not configured"] = () => {
            var config = new TestConfig();

            config.key1.should_be("value1");
            config.key2.should_be("value2");
        };

        it["adds default properties when keys don't exist"] = () => {
            var config = new TestConfig();
            config.Configure(new Properties());

            config.key1.should_be("value1");
            config.key2.should_be("value2");
        };

        it["adds missing default properties"] = () => {
            var config = new TestConfig();
            config.Configure(new Properties("key1 = newValue1\n"));

            config.key1.should_be("newValue1");
            config.key2.should_be("value2");
        };
    }
}

class TestConfig : AbstractConfigurableConfig {

    public override Dictionary<string, string> defaultProperties {
        get {
            return new Dictionary<string, string> {
                { "key1", "value1" },
                { "key2", "value2" }
            };
        }
    }

    public string key1 { 
        get { return properties["key1"]; }
    }

    public string key2 { 
        get { return properties["key2"]; }
    }
}