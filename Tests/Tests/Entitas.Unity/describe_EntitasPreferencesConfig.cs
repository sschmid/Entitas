using NSpec;
using Entitas.Unity;

class describe_EntitasPreferencesConfig : nspec {
    void when_config() {

        EntitasPreferencesConfig config = null;
        before = () => {
            config = new EntitasPreferencesConfig(string.Empty);
        };

        it["gets string from empty config"] = () => config.ToString().should_be(string.Empty);
        it["gets default value from empty config and sets value for trimmed key"] = () => {
            config.GetValueOrDefault(" testKey ", " testValue ").should_be("testValue ");
            config.ToString().should_be("testKey = testValue \n");
        };

        it["sets value for trimmed key"] = () => {
            config[" test key "] = " test value ";
            config["test key"].should_be("test value ");
            config.ToString().should_be("test key = test value \n");
        };
    }
}

