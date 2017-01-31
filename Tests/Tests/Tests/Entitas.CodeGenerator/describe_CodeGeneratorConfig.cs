using Entitas;
using Entitas.CodeGenerator;
using NSpec;

class describe_CodeGeneratorConfig : nspec {

    const string configString =
        "Entitas.CodeGenerator.TargetDirectory = path/to/folder/" + "\n" +
        "Entitas.CodeGenerator.Contexts = Core, Meta, UI" + "\n" +
        "Entitas.CodeGenerator.DataProviders = DataProvider1, DataProvider2, DataProvider3" + "\n" +
        "Entitas.CodeGenerator.CodeGenerators = Generator1, Generator2, Generator3" + "\n" +
        "Entitas.CodeGenerator.PostProcessors = PostProcessor1, PostProcessor2, PostProcessor3";

    void when_creating_config() {

        it["creates config from EntitasPreferencesConfig"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(configString), new string[0], new string[0], new string[0]);

            config.targetDirectory.should_be("path/to/folder/");
            config.contexts.should_be(new [] { "Core", "Meta", "UI" });
            config.dataProviders.should_be(new [] { "DataProvider1", "DataProvider2", "DataProvider3" });
            config.codeGenerators.should_be(new [] { "Generator1", "Generator2", "Generator3" });
            config.postProcessors.should_be(new [] { "PostProcessor1", "PostProcessor2", "PostProcessor3" });
        };

        it["gets default values when keys dont exist"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(string.Empty), new [] {"Data1, Data2"}, new [] {"Gen1, Gen2"}, new [] {"Post1, Post2"});
            config.targetDirectory.should_be("Assets/Generated/");
            config.contexts.should_be(new [] { "Game", "GameState", "Input" });
            config.dataProviders.should_be(new [] {"Data1", "Data2"});
            config.codeGenerators.should_be(new [] {"Gen1", "Gen2"});
            config.postProcessors.should_be(new [] {"Post1", "Post2"});
        };

        it["sets values"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(configString), new string[0], new string[0], new string[0]);
            config.targetDirectory = "new/path/";
            config.contexts = new [] { "Other1", "Other2" };
            config.dataProviders = new [] { "Data4", "Data5" };
            config.codeGenerators = new [] { "Generator4", "Generator5" };
            config.postProcessors = new [] { "Post4", "Post5" };

            config.targetDirectory.should_be("new/path/");
            config.contexts.should_be(new [] { "Other1", "Other2" });
            config.dataProviders.should_be(new [] { "Data4", "Data5" });
            config.codeGenerators.should_be(new [] { "Generator4", "Generator5" });
            config.postProcessors.should_be(new [] { "Post4", "Post5" });
        };

        it["gets string"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(configString), new string[0], new string[0], new string[0]);
            config.targetDirectory = "new/path/";
            config.contexts = new [] { "Other1", "Other2" };
            config.dataProviders = new [] { "Data4", "Data5" };
            config.codeGenerators = new [] { "Generator4", "Generator5" };
            config.postProcessors = new [] { "Post4", "Post5" };

            config.ToString().should_be(
                "Entitas.CodeGenerator.TargetDirectory = new/path/\n" +
                "Entitas.CodeGenerator.Contexts = Other1,Other2\n" +
                "Entitas.CodeGenerator.DataProviders = Data4,Data5\n" +
                "Entitas.CodeGenerator.CodeGenerators = Generator4,Generator5\n" +
                "Entitas.CodeGenerator.PostProcessors = Post4,Post5\n"
            );
        };

        it["gets string from empty config"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(string.Empty), new string[0], new string[0], new string[0]);
            config.ToString().should_be(
                "Entitas.CodeGenerator.TargetDirectory = Assets/Generated/\n" +
                "Entitas.CodeGenerator.Contexts = Game,GameState,Input\n" +
                "Entitas.CodeGenerator.DataProviders = \n" +
                "Entitas.CodeGenerator.CodeGenerators = \n" +
                "Entitas.CodeGenerator.PostProcessors = \n"
            );
        };

        it["removes empty contexts"] = () => {
            const string configString = "Entitas.CodeGenerator.Contexts = ,,Core,,UI,,";
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(configString), new string[0], new string[0], new string[0]);
            config.contexts.should_be(new [] { "Core", "UI" });
        };

        it["removes empty enabled code generators"] = () => {
            const string configString = "Entitas.CodeGenerator.CodeGenerators = ,,Gen1,,Gen2,,";
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(configString), new string[0], new string[0], new string[0]);
            config.codeGenerators.should_be(new [] { "Gen1", "Gen2" });
        };

        it["removes trailing comma in contexts string"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(string.Empty), new string[0], new string[0], new string[0]);
            config.contexts = new [] { "Meta", string.Empty };
            config.ToString().should_be(
                "Entitas.CodeGenerator.TargetDirectory = Assets/Generated/\n" +
                "Entitas.CodeGenerator.Contexts = Meta\n" +
                "Entitas.CodeGenerator.DataProviders = \n" +
                "Entitas.CodeGenerator.CodeGenerators = \n" +
                "Entitas.CodeGenerator.PostProcessors = \n"
            );
        };

        it["removes trailing comma in enabled code generators string"] = () => {
            var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(string.Empty), new string[0], new string[0], new string[0]);
            config.codeGenerators = new [] { "Gen1", string.Empty };
            config.ToString().should_be(
                "Entitas.CodeGenerator.TargetDirectory = Assets/Generated/\n" +
                "Entitas.CodeGenerator.Contexts = Game,GameState,Input\n" +
                "Entitas.CodeGenerator.DataProviders = \n" +
                "Entitas.CodeGenerator.CodeGenerators = Gen1\n" +
                "Entitas.CodeGenerator.PostProcessors = \n"
            );
        };
    }
}
