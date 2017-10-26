using Entitas.Utils;
using Entitas.VisualDebugging.Unity.Editor;
using NSpec;

class describe_VisualDebuggingConfig : nspec {

    const string configString =
        "Entitas.VisualDebugging.Unity.Editor.SystemWarningThreshold = 12\n" +
        "Entitas.VisualDebugging.Unity.Editor.DefaultInstanceCreatorFolderPath = dicPath\n" +
        "Entitas.VisualDebugging.Unity.Editor.TypeDrawerFolderPath = toPath\n";

    void when_creating_config() {

        VisualDebuggingConfig config = null;

        before = () => {
            config = new VisualDebuggingConfig();
        };

        context["when input string"] = () => {

            before = () => {
                config.Configure(new Preferences(new Properties(configString)));
            };

            it["creates config"] = () => {
                config.systemWarningThreshold.should_be(12);
                config.defaultInstanceCreatorFolderPath.should_be("dicPath");
                config.typeDrawerFolderPath.should_be("toPath");
            };

            context["when setting values"] = () => {

                before = () => {
                    config.systemWarningThreshold = 6;
                    config.defaultInstanceCreatorFolderPath = "newDicPath";
                    config.typeDrawerFolderPath = "newtoPath";
                };

                it["sets values"] = () => {
                    config.systemWarningThreshold.should_be(6);
                    config.defaultInstanceCreatorFolderPath.should_be("newDicPath");
                    config.typeDrawerFolderPath.should_be("newtoPath");
                };

                it["gets string"] = () => {
                    config.ToString().should_be(
                        "Entitas.VisualDebugging.Unity.Editor.SystemWarningThreshold = 6\n" +
                        "Entitas.VisualDebugging.Unity.Editor.DefaultInstanceCreatorFolderPath = newDicPath\n" +
                        "Entitas.VisualDebugging.Unity.Editor.TypeDrawerFolderPath = newtoPath\n");
                };
            };
        };
    }
}
