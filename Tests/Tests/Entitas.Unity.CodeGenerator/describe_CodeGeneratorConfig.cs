using Entitas.Unity.CodeGenerator;
using NSpec;
using Entitas.Unity;

class describe_CodeGeneratorConfig : nspec {

    const string configString =
        "Entitas.Unity.CodeGenerator.GeneratedFolderPath = path/to/folder/\n" +
        "Entitas.Unity.CodeGenerator.Pools = Core, Meta, UI\n" +
        "Entitas.Unity.CodeGenerator.EnabledCodeGenerators = Generator1, Generator2, Generator3";

    void when_creating_config() {

        it["creates config from EntitasPreferencesConfig"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(configString));

            config.generatedFolderPath.should_be("path/to/folder/");
            config.pools.should_be(new [] { "Core", "Meta", "UI" });
            config.enabledCodeGenerators.should_be(new [] { "Generator1", "Generator2", "Generator3" });
        };

        it["gets default values when keys dont exist"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(string.Empty));
            config.generatedFolderPath.should_be("Assets/Generated/");
            config.pools.should_be_empty();
            config.enabledCodeGenerators.should_be_empty();
        };

        it["sets values"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(configString));
            config.generatedFolderPath = "new/path/";
            config.pools = new [] { "Other1", "Other2" };
            config.enabledCodeGenerators = new [] { "Generator4", "Generator5" };

            config.generatedFolderPath.should_be("new/path/");
            config.pools.should_be(new [] { "Other1", "Other2" });
            config.enabledCodeGenerators.should_be(new [] { "Generator4", "Generator5" });
        };

        it["gets string"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(configString));
            config.generatedFolderPath = "new/path/";
            config.pools = new [] { "Other1", "Other2" };
            config.enabledCodeGenerators = new [] { "Generator4", "Generator5" };

            config.ToString().should_be(
                "Entitas.Unity.CodeGenerator.GeneratedFolderPath = new/path/\n" +
                "Entitas.Unity.CodeGenerator.Pools = Other1,Other2\n" +
                "Entitas.Unity.CodeGenerator.EnabledCodeGenerators = Generator4,Generator5\n");
        };

        it["gets string from empty config"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(string.Empty));
            config.ToString().should_be(
                "Entitas.Unity.CodeGenerator.GeneratedFolderPath = Assets/Generated/\n" +
                "Entitas.Unity.CodeGenerator.Pools = \n" +
                "Entitas.Unity.CodeGenerator.EnabledCodeGenerators = \n");
        };

        it["removes empty pools"] = () => {
            const string configString = "Entitas.Unity.CodeGenerator.Pools = ,,Core,,UI,,";
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(configString));
            config.pools.should_be(new [] { "Core", "UI" });
        };

        it["removes empty enabled code generators"] = () => {
            const string configString = "Entitas.Unity.CodeGenerator.EnabledCodeGenerators = ,,Gen1,,Gen2,,";
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(configString));
            config.enabledCodeGenerators.should_be(new [] { "Gen1", "Gen2" });
        };

        it["removes trailing comma in pools string"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(string.Empty));
            config.pools = new [] { "Meta", string.Empty };
            config.ToString().should_be(
                "Entitas.Unity.CodeGenerator.GeneratedFolderPath = Assets/Generated/\n" +
                "Entitas.Unity.CodeGenerator.Pools = Meta\n" +
                "Entitas.Unity.CodeGenerator.EnabledCodeGenerators = \n"
            );
        };

        it["removes trailing comma in enabled code generators string"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(string.Empty));
            config.enabledCodeGenerators = new [] { "Gen1", string.Empty };
            config.ToString().should_be(
                "Entitas.Unity.CodeGenerator.GeneratedFolderPath = Assets/Generated/\n" +
                "Entitas.Unity.CodeGenerator.Pools = \n" +
                "Entitas.Unity.CodeGenerator.EnabledCodeGenerators = Gen1\n"
            );
        };
    }
}

