using Entitas.CodeGeneration.CodeGenerator;
using Entitas.Utils;
using NSpec;

class describe_CodeGeneratorConfig : nspec {

    const string configString =
        "Entitas.CodeGeneration.SearchPaths = sp1, sp2" + "\n" +
        "Entitas.CodeGeneration.Assemblies = a1, a2" + "\n" +
        "Entitas.CodeGeneration.Plugins = p1, p2" + "\n" +

        "Entitas.CodeGeneration.DataProviders = dp1,dp2,dp3" + "\n" +
        "Entitas.CodeGeneration.CodeGenerators = cg1, cg2, cg3" + "\n" +
        "Entitas.CodeGeneration.PostProcessors = pp1 , pp2 , pp3" + "\n";

    void when_creating_config() {

        CodeGeneratorConfig config = null;

        before = () => {
            config = new CodeGeneratorConfig();
        };

        context["when input is empty"] = () => {

            before = () => {
                config.Configure(new Properties());
            };

            it["gets default values"] = () => {
                config.searchPaths.should_be(new [] {
                    "Libraries/Entitas", "Libraries/Entitas/Editor",
                    "/Applications/Unity/Unity.app/Contents/Managed",
                    "/Applications/Unity/Unity.app/Contents/Mono/lib/mono/unity" });

                config.assemblies.should_be(new [] { "Library/ScriptAssemblies/Assembly-CSharp.dll"});

                config.plugins.should_be(new [] {
                    "Entitas.CodeGeneration.Plugins",
                    "Entitas.VisualDebugging.CodeGeneration.Plugins",
                    "Entitas.Blueprints.CodeGeneration.Plugins"});

                config.dataProviders.should_be_empty();
                config.codeGenerators.should_be_empty();
                config.postProcessors.should_be_empty();
            };

            it["gets string from empty config"] = () => {
                config.ToString().should_be(
                    "Entitas.CodeGeneration.SearchPaths = Libraries/Entitas, Libraries/Entitas/Editor, /Applications/Unity/Unity.app/Contents/Managed, /Applications/Unity/Unity.app/Contents/Mono/lib/mono/unity\n" +
                    "Entitas.CodeGeneration.Assemblies = Library/ScriptAssemblies/Assembly-CSharp.dll\n" +
                    "Entitas.CodeGeneration.Plugins = Entitas.CodeGeneration.Plugins, Entitas.VisualDebugging.CodeGeneration.Plugins, Entitas.Blueprints.CodeGeneration.Plugins\n" +
                    "Entitas.CodeGeneration.DataProviders = \n" +
                    "Entitas.CodeGeneration.CodeGenerators = \n" +
                    "Entitas.CodeGeneration.PostProcessors = \n"
                );
            };
        };

        context["when input string"] = () => {

            before = () => {
                config.Configure(new Properties(configString));
            };

            it["creates config"] = () => {
                config.searchPaths.should_be(new [] { "s1", "s2"});
                config.assemblies.should_be(new [] { "a1", "a2"});
                config.plugins.should_be(new [] { "cg1", "cg2"});

                config.dataProviders.should_be(new [] { "dp1", "dp2", "dp3" });
                config.codeGenerators.should_be(new [] { "cg1", "cg2", "cg3" });
                config.postProcessors.should_be(new [] { "pp1", "pp2", "pp3" });
            };

            context["when setting values"] = () => {

                before = () => {
                    config.searchPaths = new [] { "newS1", "newS2"};
                    config.assemblies = new [] { "newA1", "newA2"};
                    config.plugins = new [] { "newP1", "newP2"};

                    config.dataProviders = new [] { "newDp1", "newDp2" };
                    config.codeGenerators = new [] { "newCg1", "newCg2" };
                    config.postProcessors = new [] { "newPp1", "newPp2" };
                };

                it["has updated values"] = () => {
                    config.searchPaths.should_be(new [] { "newS1", "newS2"});
                    config.assemblies.should_be(new [] { "newA1", "newA2"});
                    config.plugins.should_be(new [] { "newP1", "newP2"});

                    config.dataProviders.should_be(new [] { "newDp1", "newDp2" });
                    config.codeGenerators.should_be(new [] { "newCg1", "newCg2" });
                    config.postProcessors.should_be(new [] { "newPp1", "newPp2" });
                };

                it["gets string"] = () => {
                    config.ToString().should_be(
                        "Entitas.CodeGeneration.SearchPaths = newS1, newS2\n" +
                        "Entitas.CodeGeneration.Assemblies = newA1, newA2\n" +
                        "Entitas.CodeGeneration.Plugins = newP1, newP2\n" +

                        "Entitas.CodeGeneration.DataProviders = newDp1, newDp2\n" +
                        "Entitas.CodeGeneration.CodeGenerators = newCg1, newCg2\n" +
                        "Entitas.CodeGeneration.PostProcessors = newPp1, newPp2"
                    );
                };
            };
        };
    }
}
