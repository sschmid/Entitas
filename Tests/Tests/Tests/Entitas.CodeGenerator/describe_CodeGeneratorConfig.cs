using Entitas.CodeGeneration.CodeGenerator;
using Entitas.Utils;
using NSpec;

class describe_CodeGeneratorConfig : nspec {

    const string configString =
        "Entitas.CodeGeneration.SearchPaths = base1, base/2/" + "\n" +
        "Entitas.CodeGeneration.Assemblies = game.dll, kit.dll" + "\n" +
        "Entitas.CodeGeneration.Plugins = gen1.dll, gen2.dll" + "\n" +
        "Entitas.CodeGeneration.DataProviders = DataProvider1,DataProvider2,DataProvider3" + "\n" +
        "Entitas.CodeGeneration.CodeGenerators = Generator1, Generator2, Generator3" + "\n" +
        "Entitas.CodeGeneration.PostProcessors = PostProcessor1 , PostProcessor2 , PostProcessor3" + "\n" +
        "Entitas.CodeGeneration.TargetDirectory = path/to/folder/" + "\n" +
        "Entitas.CodeGeneration.Contexts = Core, Meta, UI" + "\n";

    void when_creating_config() {

        it["creates config from EntitasPreferencesConfig"] = () => {
            var config = new CodeGeneratorConfig(new Config(configString));

            config.searchPaths.should_be(new [] { "base1", "base/2/"});
            config.assemblies.should_be(new [] { "game.dll", "kit.dll"});
            config.plugins.should_be(new [] { "gen1.dll", "gen2.dll"});
            config.dataProviders.should_be(new [] { "DataProvider1", "DataProvider2", "DataProvider3" });
            config.codeGenerators.should_be(new [] { "Generator1", "Generator2", "Generator3" });
            config.postProcessors.should_be(new [] { "PostProcessor1", "PostProcessor2", "PostProcessor3" });
            config.targetDirectory.should_be("path/to/folder/");
            config.contexts.should_be(new [] { "Core", "Meta", "UI" });
        };

        it["gets default values when keys dont exist"] = () => {
            var config = new CodeGeneratorConfig(new Config(string.Empty), new [] {"Data1, Data2"}, new [] {"Gen1, Gen2"}, new [] {"Post1, Post2"});
            config.searchPaths.should_be(new [] { "Libraries/Entitas", "Libraries/Entitas/Editor", "/Applications/Unity/Unity.app/Contents/Managed", "/Applications/Unity/Unity.app/Contents/Mono/lib/mono/unity" });
            config.assemblies.should_be(new [] { "Library/ScriptAssemblies/Assembly-CSharp.dll"});
            config.plugins.should_be(new [] { "Entitas.CodeGeneration.Plugins", "Entitas.VisualDebugging.CodeGeneration.Plugins", "Entitas.Blueprints.CodeGeneration.Plugins"});
            config.dataProviders.should_be(new [] {"Data1", "Data2"});
            config.codeGenerators.should_be(new [] {"Gen1", "Gen2"});
            config.postProcessors.should_be(new [] {"Post1", "Post2"});
            config.targetDirectory.should_be("Assets/Generated");
            config.contexts.should_be(new [] { "Game", "GameState", "Input" });
        };

        it["sets values"] = () => {
            var config = new CodeGeneratorConfig(new Config(configString), new string[0], new string[0], new string[0]);
            config.searchPaths = new [] { "newBase1", "newBase2"};
            config.assemblies = new [] { "game.dll", "physics.dll"};
            config.plugins = new [] { "myGen1.dll", "myGen2.dll"};
            config.dataProviders = new [] { "Data4", "Data5" };
            config.codeGenerators = new [] { "Generator4", "Generator5" };
            config.postProcessors = new [] { "Post4", "Post5" };
            config.targetDirectory = "new/path/";
            config.contexts = new [] { "Other1", "Other2" };

            config.searchPaths.should_be(new [] { "newBase1", "newBase2"});
            config.assemblies.should_be(new [] { "game.dll", "physics.dll"});
            config.plugins.should_be(new [] { "myGen1.dll", "myGen2.dll"});
            config.dataProviders.should_be(new [] { "Data4", "Data5" });
            config.codeGenerators.should_be(new [] { "Generator4", "Generator5" });
            config.postProcessors.should_be(new [] { "Post4", "Post5" });
            config.targetDirectory.should_be("new/path/");
            config.contexts.should_be(new [] { "Other1", "Other2" });
        };

        it["gets string"] = () => {
            var config = new CodeGeneratorConfig(new Config(configString));
            config.searchPaths = new [] { "newBase1", "newBase2"};
            config.assemblies = new [] { "game.dll", "physics.dll"};
            config.plugins = new [] { "myGen1.dll", "myGen2.dll"};
            config.dataProviders = new [] { "Data4", "Data5" };
            config.codeGenerators = new [] { "Generator4", "Generator5" };
            config.postProcessors = new [] { "Post4", "Post5" };
            config.targetDirectory = "new/path/";
            config.contexts = new [] { "Other1", "Other2" };

            config.ToString().should_be(
                "Entitas.CodeGeneration.SearchPaths = newBase1, newBase2\n" +
                "Entitas.CodeGeneration.Assemblies = game.dll, physics.dll\n" +
                "Entitas.CodeGeneration.Plugins = myGen1.dll, myGen2.dll\n" +
                "Entitas.CodeGeneration.DataProviders = Data4, Data5\n" +
                "Entitas.CodeGeneration.CodeGenerators = Generator4, Generator5\n" +
                "Entitas.CodeGeneration.PostProcessors = Post4, Post5\n" +
                "Entitas.CodeGeneration.TargetDirectory = new/path/\n" +
                "Entitas.CodeGeneration.Contexts = Other1, Other2\n"
            );
        };

        it["gets string from empty config"] = () => {
            var config = new CodeGeneratorConfig(new Config(string.Empty));
            config.ToString().should_be(
                "Entitas.CodeGeneration.SearchPaths = Libraries/Entitas, Libraries/Entitas/Editor, /Applications/Unity/Unity.app/Contents/Managed, /Applications/Unity/Unity.app/Contents/Mono/lib/mono/unity\n" +
                "Entitas.CodeGeneration.Assemblies = Library/ScriptAssemblies/Assembly-CSharp.dll\n" +
                "Entitas.CodeGeneration.Plugins = Entitas.CodeGeneration.Plugins, Entitas.VisualDebugging.CodeGeneration.Plugins, Entitas.Blueprints.CodeGeneration.Plugins\n" +
                "Entitas.CodeGeneration.DataProviders = \n" +
                "Entitas.CodeGeneration.CodeGenerators = \n" +
                "Entitas.CodeGeneration.PostProcessors = \n" +
                "Entitas.CodeGeneration.TargetDirectory = Assets/Generated\n" +
                "Entitas.CodeGeneration.Contexts = Game, GameState, Input\n"
            );
        };

        it["removes empty entries"] = () => {
            const string configString = "Entitas.CodeGeneration.Contexts = ,,Core,,UI,,";
            var config = new CodeGeneratorConfig(new Config(configString));
            config.contexts.should_be(new [] { "Core", "UI" });
        };

        it["removes trailing comma"] = () => {
            var config = new CodeGeneratorConfig(new Config(string.Empty));
            config.contexts = new [] { "Meta", string.Empty };
            config.ToString().should_be(
                "Entitas.CodeGeneration.SearchPaths = Libraries/Entitas, Libraries/Entitas/Editor, /Applications/Unity/Unity.app/Contents/Managed, /Applications/Unity/Unity.app/Contents/Mono/lib/mono/unity\n" +
                "Entitas.CodeGeneration.Assemblies = Library/ScriptAssemblies/Assembly-CSharp.dll\n" +
                "Entitas.CodeGeneration.Plugins = Entitas.CodeGeneration.Plugins, Entitas.VisualDebugging.CodeGeneration.Plugins, Entitas.Blueprints.CodeGeneration.Plugins\n" +
                "Entitas.CodeGeneration.DataProviders = \n" +
                "Entitas.CodeGeneration.CodeGenerators = \n" +
                "Entitas.CodeGeneration.PostProcessors = \n" +
                "Entitas.CodeGeneration.TargetDirectory = Assets/Generated\n" +
                "Entitas.CodeGeneration.Contexts = Meta\n"
            );
        };

        it["has all keys"] = () => {
            var keys = CodeGeneratorConfig.keys;
            keys.should_contain("Entitas.CodeGeneration.SearchPaths");
            keys.should_contain("Entitas.CodeGeneration.Assemblies");
            keys.should_contain("Entitas.CodeGeneration.Plugins");
            keys.should_contain("Entitas.CodeGeneration.DataProviders");
            keys.should_contain("Entitas.CodeGeneration.CodeGenerators");
            keys.should_contain("Entitas.CodeGeneration.PostProcessors");
            keys.should_contain("Entitas.CodeGeneration.TargetDirectory");
            keys.should_contain("Entitas.CodeGeneration.Contexts");
        };
    }
}
