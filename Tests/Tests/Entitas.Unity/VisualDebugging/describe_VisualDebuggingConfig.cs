using Entitas.Unity.VisualDebugging;
using Entitas.Utils;
using NSpec;

class describe_VisualDebuggingConfig : nspec {

    const string configString =
        "Entitas.Unity.VisualDebugging.DefaultInstanceCreatorFolderPath = path/to/folder/\n" +
        "Entitas.Unity.VisualDebugging.TypeDrawerFolderPath = path/to/otherFolder/\n";
    
    void when_creating_config() {
        
        it["creates config from EntitasPreferencesConfig"] = () => {
            var config = new VisualDebuggingConfig(new EntitasPreferencesConfig(configString));
            config.defaultInstanceCreatorFolderPath.should_be("path/to/folder/");
            config.typeDrawerFolderPath.should_be("path/to/otherFolder/");
        };

        it["gets default values when keys dont exist"] = () => {
            var config = new VisualDebuggingConfig(new EntitasPreferencesConfig(string.Empty));
            config.defaultInstanceCreatorFolderPath.should_be("Assets/Editor/DefaultInstanceCreator/");
            config.typeDrawerFolderPath.should_be("Assets/Editor/TypeDrawer/");
        };

        it["sets values"] = () => {
            var config = new VisualDebuggingConfig(new EntitasPreferencesConfig(configString));
            config.defaultInstanceCreatorFolderPath = "new/path/";
            config.typeDrawerFolderPath = "new/otherPath/";

            config.defaultInstanceCreatorFolderPath.should_be("new/path/");
            config.typeDrawerFolderPath.should_be("new/otherPath/");
        };

        it["gets string"] = () => {
            var config = new VisualDebuggingConfig(new EntitasPreferencesConfig(configString));
            config.defaultInstanceCreatorFolderPath = "new/path/";
            config.typeDrawerFolderPath = "new/otherPath/";

            config.ToString().should_be(
                "Entitas.Unity.VisualDebugging.DefaultInstanceCreatorFolderPath = new/path/\n" +
                "Entitas.Unity.VisualDebugging.TypeDrawerFolderPath = new/otherPath/\n");
        };

        it["gets string from empty config"] = () => {
            var config = new VisualDebuggingConfig(new EntitasPreferencesConfig(string.Empty));
            config.ToString().should_be(
                "Entitas.Unity.VisualDebugging.DefaultInstanceCreatorFolderPath = Assets/Editor/DefaultInstanceCreator/\n" +
                "Entitas.Unity.VisualDebugging.TypeDrawerFolderPath = Assets/Editor/TypeDrawer/\n");
        };
    }
}
