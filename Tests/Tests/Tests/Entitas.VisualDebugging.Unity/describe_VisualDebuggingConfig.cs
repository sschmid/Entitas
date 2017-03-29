using Entitas;
using Entitas.VisualDebugging.Unity;
using NSpec;

class describe_VisualDebuggingConfig : nspec {

    const string configString =
        "Entitas.VisualDebugging.Unity.SystemWarningThreshold = 12\n" +
        "Entitas.VisualDebugging.Unity.DefaultInstanceCreatorFolderPath = path/to/folder/\n" +
        "Entitas.VisualDebugging.Unity.TypeDrawerFolderPath = path/to/otherFolder/\n";
    
    void when_creating_config() {
        
        it["creates config from EntitasPreferencesConfig"] = () => {
            var config = new VisualDebuggingConfig(new EntitasPreferencesConfig(configString));
            config.systemWarningThreshold.should_be("12");
            config.defaultInstanceCreatorFolderPath.should_be("path/to/folder/");
            config.typeDrawerFolderPath.should_be("path/to/otherFolder/");
        };

        it["gets default values when keys dont exist"] = () => {
            var config = new VisualDebuggingConfig(new EntitasPreferencesConfig(string.Empty));
            config.systemWarningThreshold.should_be("8");
            config.defaultInstanceCreatorFolderPath.should_be("Assets/Editor/DefaultInstanceCreator/");
            config.typeDrawerFolderPath.should_be("Assets/Editor/TypeDrawer/");
        };

        it["sets values"] = () => {
            var config = new VisualDebuggingConfig(new EntitasPreferencesConfig(configString));
            config.systemWarningThreshold = "6";
            config.defaultInstanceCreatorFolderPath = "new/path/";
            config.typeDrawerFolderPath = "new/otherPath/";

            config.systemWarningThreshold.should_be("6");
            config.defaultInstanceCreatorFolderPath.should_be("new/path/");
            config.typeDrawerFolderPath.should_be("new/otherPath/");
        };

        it["gets string"] = () => {
            var config = new VisualDebuggingConfig(new EntitasPreferencesConfig(configString));
            config.systemWarningThreshold = "6";
            config.defaultInstanceCreatorFolderPath = "new/path/";
            config.typeDrawerFolderPath = "new/otherPath/";

            config.ToString().should_be(
                "Entitas.VisualDebugging.Unity.SystemWarningThreshold = 6\n" +
                "Entitas.VisualDebugging.Unity.DefaultInstanceCreatorFolderPath = new/path/\n" +
                "Entitas.VisualDebugging.Unity.TypeDrawerFolderPath = new/otherPath/\n");
        };

        it["gets string from empty config"] = () => {
            var config = new VisualDebuggingConfig(new EntitasPreferencesConfig(string.Empty));
            config.ToString().should_be(
                "Entitas.VisualDebugging.Unity.SystemWarningThreshold = 8\n" +
                "Entitas.VisualDebugging.Unity.DefaultInstanceCreatorFolderPath = Assets/Editor/DefaultInstanceCreator/\n" +
                "Entitas.VisualDebugging.Unity.TypeDrawerFolderPath = Assets/Editor/TypeDrawer/\n");
        };

        it["has all keys"] = () => {
            var keys = VisualDebuggingConfig.keys;
            keys.should_contain("Entitas.VisualDebugging.Unity.SystemWarningThreshold");
            keys.should_contain("Entitas.VisualDebugging.Unity.DefaultInstanceCreatorFolderPath");
            keys.should_contain("Entitas.VisualDebugging.Unity.TypeDrawerFolderPath");
        };
    }
}
