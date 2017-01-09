using Entitas.Serialization.Configuration;
using NSpec;

class describe_CodeGeneratorConfig : nspec {

    const string configString =
        "Entitas.CodeGenerator.GeneratedFolderPath = path/to/folder/\n" +
        "Entitas.CodeGenerator.Contexts = Core, Meta, UI\n" +
        "Entitas.CodeGenerator.EnabledCodeGenerators = Generator1, Generator2, Generator3";

    void when_creating_config() {

        it["creates config from EntitasPreferencesConfig"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(configString), new string[0]);

            config.generatedFolderPath.should_be("path/to/folder/");
            config.contexts.should_be(new [] { "Core", "Meta", "UI" });
            config.enabledCodeGenerators.should_be(new [] { "Generator1", "Generator2", "Generator3" });
        };

        it["gets default values when keys dont exist"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(string.Empty), new [] {"Gen1, Gen2"});
            config.generatedFolderPath.should_be("Assets/Generated/");
            config.contexts.should_be_empty();
            config.enabledCodeGenerators.should_be(new [] {"Gen1", "Gen2"});
        };

        it["sets values"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(configString), new string[0]);
            config.generatedFolderPath = "new/path/";
            config.contexts = new [] { "Other1", "Other2" };
            config.enabledCodeGenerators = new [] { "Generator4", "Generator5" };

            config.generatedFolderPath.should_be("new/path/");
            config.contexts.should_be(new [] { "Other1", "Other2" });
            config.enabledCodeGenerators.should_be(new [] { "Generator4", "Generator5" });
        };

        it["gets string"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(configString), new string[0]);
            config.generatedFolderPath = "new/path/";
            config.contexts = new [] { "Other1", "Other2" };
            config.enabledCodeGenerators = new [] { "Generator4", "Generator5" };

            config.ToString().should_be(
                "Entitas.CodeGenerator.GeneratedFolderPath = new/path/\n" +
                "Entitas.CodeGenerator.Contexts = Other1,Other2\n" +
                "Entitas.CodeGenerator.EnabledCodeGenerators = Generator4,Generator5\n");
        };

        it["gets string from empty config"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(string.Empty), new string[0]);
            config.ToString().should_be(
                "Entitas.CodeGenerator.GeneratedFolderPath = Assets/Generated/\n" +
                "Entitas.CodeGenerator.Contexts = \n" +
                "Entitas.CodeGenerator.EnabledCodeGenerators = \n");
        };

        it["removes empty contexts"] = () => {
            const string configString = "Entitas.CodeGenerator.Contexts = ,,Core,,UI,,";
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(configString), new string[0]);
            config.contexts.should_be(new [] { "Core", "UI" });
        };

        it["removes empty enabled code generators"] = () => {
            const string configString = "Entitas.CodeGenerator.EnabledCodeGenerators = ,,Gen1,,Gen2,,";
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(configString), new string[0]);
            config.enabledCodeGenerators.should_be(new [] { "Gen1", "Gen2" });
        };

        it["removes trailing comma in contexts string"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(string.Empty), new string[0]);
            config.contexts = new [] { "Meta", string.Empty };
            config.ToString().should_be(
                "Entitas.CodeGenerator.GeneratedFolderPath = Assets/Generated/\n" +
                "Entitas.CodeGenerator.Contexts = Meta\n" +
                "Entitas.CodeGenerator.EnabledCodeGenerators = \n"
            );
        };

        it["removes trailing comma in enabled code generators string"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(string.Empty), new string[0]);
            config.enabledCodeGenerators = new [] { "Gen1", string.Empty };
            config.ToString().should_be(
                "Entitas.CodeGenerator.GeneratedFolderPath = Assets/Generated/\n" +
                "Entitas.CodeGenerator.Contexts = \n" +
                "Entitas.CodeGenerator.EnabledCodeGenerators = Gen1\n"
            );
        };
    }
}
