using Entitas.CodeGeneration.CodeGenerator;
using Entitas.Utils;
using NSpec;

class describe_CodeGeneratorConfig : nspec {

    const string configString =
        "Entitas.CodeGenerator.Project = path/to/project/" + "\n" +
        "Entitas.CodeGenerator.AssemblyBasePaths = base1, base/2/" + "\n" +
        "Entitas.CodeGenerator.Assemblies = game.dll, kit.dll" + "\n" +
        "Entitas.CodeGenerator.CodeGeneratorAssemblies = gen1.dll, gen2.dll" + "\n" +
        "Entitas.CodeGenerator.TargetDirectory = path/to/folder/" + "\n" +
        "Entitas.CodeGenerator.Contexts = Core, Meta, UI" + "\n" +
        "Entitas.CodeGenerator.DataProviders = DataProvider1,DataProvider2,DataProvider3" + "\n" +
        "Entitas.CodeGenerator.CodeGenerators = Generator1, Generator2, Generator3" + "\n" +
        "Entitas.CodeGenerator.PostProcessors = PostProcessor1 , PostProcessor2 , PostProcessor3";

    void when_creating_config() {

        it["creates config from EntitasPreferencesConfig"] = () => {
            var config = new CodeGeneratorConfig(new Config(configString));

            config.projectPath.should_be("path/to/project/");
            config.assemblyBasePaths.should_be(new [] { "base1", "base/2/"});
            config.assemblyPaths.should_be(new [] { "game.dll", "kit.dll"});
            config.codeGeneratorAssemblyPaths.should_be(new [] { "gen1.dll", "gen2.dll"});
            config.targetDirectory.should_be("path/to/folder/");
            config.contexts.should_be(new [] { "Core", "Meta", "UI" });
            config.dataProviders.should_be(new [] { "DataProvider1", "DataProvider2", "DataProvider3" });
            config.codeGenerators.should_be(new [] { "Generator1", "Generator2", "Generator3" });
            config.postProcessors.should_be(new [] { "PostProcessor1", "PostProcessor2", "PostProcessor3" });
        };

        it["gets default values when keys dont exist"] = () => {
            var config = new CodeGeneratorConfig(new Config(string.Empty), new [] {"Data1, Data2"}, new [] {"Gen1, Gen2"}, new [] {"Post1, Post2"});
            config.projectPath.should_be("Assembly-CSharp.csproj");
            config.assemblyBasePaths.should_be(new [] { "/Applications/Unity/Unity.app/Contents/Managed", "/Applications/Unity/Unity.app/Contents/Mono/lib/mono/unity" });
            config.assemblyPaths.should_be(new [] { "Library/ScriptAssemblies/Assembly-CSharp.dll"});
            config.codeGeneratorAssemblyPaths.should_be(new [] { "Library/ScriptAssemblies/Assembly-CSharp-Editor.dll"});
            config.targetDirectory.should_be("Assets/Generated/");
            config.contexts.should_be(new [] { "Game", "GameState", "Input" });
            config.dataProviders.should_be(new [] {"Data1", "Data2"});
            config.codeGenerators.should_be(new [] {"Gen1", "Gen2"});
            config.postProcessors.should_be(new [] {"Post1", "Post2"});
        };

        it["sets values"] = () => {
            var config = new CodeGeneratorConfig(new Config(configString), new string[0], new string[0], new string[0]);
            config.projectPath = "path/to/newProject/";
            config.assemblyBasePaths = new [] { "newBase1", "newBase2"};
            config.assemblyPaths = new [] { "game.dll", "physics.dll"};
            config.codeGeneratorAssemblyPaths = new [] { "myGen1.dll", "myGen2.dll"};
            config.targetDirectory = "new/path/";
            config.contexts = new [] { "Other1", "Other2" };
            config.dataProviders = new [] { "Data4", "Data5" };
            config.codeGenerators = new [] { "Generator4", "Generator5" };
            config.postProcessors = new [] { "Post4", "Post5" };

            config.projectPath.should_be("path/to/newProject/");
            config.assemblyBasePaths.should_be(new [] { "newBase1", "newBase2"});
            config.assemblyPaths.should_be(new [] { "game.dll", "physics.dll"});
            config.codeGeneratorAssemblyPaths.should_be(new [] { "myGen1.dll", "myGen2.dll"});
            config.targetDirectory.should_be("new/path/");
            config.contexts.should_be(new [] { "Other1", "Other2" });
            config.dataProviders.should_be(new [] { "Data4", "Data5" });
            config.codeGenerators.should_be(new [] { "Generator4", "Generator5" });
            config.postProcessors.should_be(new [] { "Post4", "Post5" });
        };

        it["gets string"] = () => {
            var config = new CodeGeneratorConfig(new Config(configString));
            config.projectPath = "path/to/newProject/";
            config.assemblyBasePaths = new [] { "newBase1", "newBase2"};
            config.assemblyPaths = new [] { "game.dll", "physics.dll"};
            config.codeGeneratorAssemblyPaths = new [] { "myGen1.dll", "myGen2.dll"};
            config.targetDirectory = "new/path/";
            config.contexts = new [] { "Other1", "Other2" };
            config.dataProviders = new [] { "Data4", "Data5" };
            config.codeGenerators = new [] { "Generator4", "Generator5" };
            config.postProcessors = new [] { "Post4", "Post5" };

            config.ToString().should_be(
                "Entitas.CodeGenerator.Project = path/to/newProject/\n" +
                "Entitas.CodeGenerator.AssemblyBasePaths = newBase1, newBase2\n" +
                "Entitas.CodeGenerator.Assemblies = game.dll, physics.dll\n" +
                "Entitas.CodeGenerator.CodeGeneratorAssemblies = myGen1.dll, myGen2.dll\n" +
                "Entitas.CodeGenerator.TargetDirectory = new/path/\n" +
                "Entitas.CodeGenerator.Contexts = Other1, Other2\n" +
                "Entitas.CodeGenerator.DataProviders = Data4, Data5\n" +
                "Entitas.CodeGenerator.CodeGenerators = Generator4, Generator5\n" +
                "Entitas.CodeGenerator.PostProcessors = Post4, Post5\n"
            );
        };

        it["gets string from empty config"] = () => {
            var config = new CodeGeneratorConfig(new Config(string.Empty));
            config.ToString().should_be(
                "Entitas.CodeGenerator.Project = Assembly-CSharp.csproj\n" +
                "Entitas.CodeGenerator.AssemblyBasePaths = /Applications/Unity/Unity.app/Contents/Managed, /Applications/Unity/Unity.app/Contents/Mono/lib/mono/unity\n" +
                "Entitas.CodeGenerator.Assemblies = Library/ScriptAssemblies/Assembly-CSharp.dll\n" +
                "Entitas.CodeGenerator.CodeGeneratorAssemblies = Library/ScriptAssemblies/Assembly-CSharp-Editor.dll\n" +
                "Entitas.CodeGenerator.TargetDirectory = Assets/Generated/\n" +
                "Entitas.CodeGenerator.Contexts = Game, GameState, Input\n" +
                "Entitas.CodeGenerator.DataProviders = \n" +
                "Entitas.CodeGenerator.CodeGenerators = \n" +
                "Entitas.CodeGenerator.PostProcessors = \n"
            );
        };

        it["removes empty entries"] = () => {
            const string configString = "Entitas.CodeGenerator.Contexts = ,,Core,,UI,,";
            var config = new CodeGeneratorConfig(new Config(configString));
            config.contexts.should_be(new [] { "Core", "UI" });
        };

        it["removes trailing comma"] = () => {
            var config = new CodeGeneratorConfig(new Config(string.Empty));
            config.contexts = new [] { "Meta", string.Empty };
            config.ToString().should_be(
                "Entitas.CodeGenerator.Project = Assembly-CSharp.csproj\n" +
                "Entitas.CodeGenerator.AssemblyBasePaths = /Applications/Unity/Unity.app/Contents/Managed, /Applications/Unity/Unity.app/Contents/Mono/lib/mono/unity\n" +
                "Entitas.CodeGenerator.Assemblies = Library/ScriptAssemblies/Assembly-CSharp.dll\n" +
                "Entitas.CodeGenerator.CodeGeneratorAssemblies = Library/ScriptAssemblies/Assembly-CSharp-Editor.dll\n" +
                "Entitas.CodeGenerator.TargetDirectory = Assets/Generated/\n" +
                "Entitas.CodeGenerator.Contexts = Meta\n" +
                "Entitas.CodeGenerator.DataProviders = \n" +
                "Entitas.CodeGenerator.CodeGenerators = \n" +
                "Entitas.CodeGenerator.PostProcessors = \n"
            );
        };

        it["has all keys"] = () => {
            var keys = CodeGeneratorConfig.keys;
            keys.should_contain("Entitas.CodeGenerator.Project");
            keys.should_contain("Entitas.CodeGenerator.AssemblyBasePaths");
            keys.should_contain("Entitas.CodeGenerator.Assemblies");
            keys.should_contain("Entitas.CodeGenerator.CodeGeneratorAssemblies");
            keys.should_contain("Entitas.CodeGenerator.TargetDirectory");
            keys.should_contain("Entitas.CodeGenerator.Contexts");
            keys.should_contain("Entitas.CodeGenerator.DataProviders");
            keys.should_contain("Entitas.CodeGenerator.CodeGenerators");
            keys.should_contain("Entitas.CodeGenerator.PostProcessors");
        };
    }
}
