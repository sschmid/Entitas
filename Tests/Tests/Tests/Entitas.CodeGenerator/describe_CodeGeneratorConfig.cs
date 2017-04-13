using Entitas.CodeGeneration.CodeGenerator;
using Entitas.Utils;
using NSpec;

class describe_CodeGeneratorConfig : nspec {

    const string configString =
        "Entitas.CodeGeneration.CodeGenerator.SearchPaths = sp1, sp2" + "\n" +
        "Entitas.CodeGeneration.CodeGenerator.Plugins = p1, p2" + "\n" +

        "Entitas.CodeGeneration.CodeGenerator.DataProviders = dp1,dp2,dp3" + "\n" +
        "Entitas.CodeGeneration.CodeGenerator.CodeGenerators = cg1, cg2, cg3" + "\n" +
        "Entitas.CodeGeneration.CodeGenerator.PostProcessors = pp1 , pp2 , pp3" + "\n";

    void when_creating_config() {

        CodeGeneratorConfig config = null;

        before = () => {
            config = new CodeGeneratorConfig();
        };

        context["when not configured"] = () => {

            it["gets default values"] = () => {
                config.searchPaths.should_be(new [] {
                    "Assets/Libraries/Entitas", "Assets/Libraries/Entitas/Editor",
                    "/Applications/Unity/Unity.app/Contents/Managed",
                    "/Applications/Unity/Unity.app/Contents/Mono/lib/mono/unity" });

                config.plugins.should_be(new [] {
                    "Entitas.CodeGeneration.Plugins",
                    "Entitas.VisualDebugging.CodeGeneration.Plugins",
                    "Entitas.Blueprints.CodeGeneration.Plugins"});

                config.dataProviders.should_be_empty();
                config.codeGenerators.should_be_empty();
                config.postProcessors.should_be_empty();
            };

            it["gets string with default values"] = () => {
                config.ToString().should_be(
                    "Entitas.CodeGeneration.CodeGenerator.SearchPaths = Assets/Libraries/Entitas, Assets/Libraries/Entitas/Editor, /Applications/Unity/Unity.app/Contents/Managed, /Applications/Unity/Unity.app/Contents/Mono/lib/mono/unity\n" +
                    "Entitas.CodeGeneration.CodeGenerator.Plugins = Entitas.CodeGeneration.Plugins, Entitas.VisualDebugging.CodeGeneration.Plugins, Entitas.Blueprints.CodeGeneration.Plugins\n" +
                    "Entitas.CodeGeneration.CodeGenerator.DataProviders = \n" +
                    "Entitas.CodeGeneration.CodeGenerator.CodeGenerators = \n" +
                    "Entitas.CodeGeneration.CodeGenerator.PostProcessors = \n"
                );
            };
        };

        context["when input string"] = () => {

            before = () => {
                config.Configure(new Properties(configString));
            };

            it["creates config"] = () => {
                config.searchPaths.should_be(new [] { "sp1", "sp2"});
                config.plugins.should_be(new [] { "p1", "p2"});

                config.dataProviders.should_be(new [] { "dp1", "dp2", "dp3" });
                config.codeGenerators.should_be(new [] { "cg1", "cg2", "cg3" });
                config.postProcessors.should_be(new [] { "pp1", "pp2", "pp3" });
            };

            context["when setting values"] = () => {

                before = () => {
                    config.searchPaths = new [] { "newS1", "newS2"};
                    config.plugins = new [] { "newP1", "newP2"};

                    config.dataProviders = new [] { "newDp1", "newDp2" };
                    config.codeGenerators = new [] { "newCg1", "newCg2" };
                    config.postProcessors = new [] { "newPp1", "newPp2" };
                };

                it["has updated values"] = () => {
                    config.searchPaths.should_be(new [] { "newS1", "newS2"});
                    config.plugins.should_be(new [] { "newP1", "newP2"});

                    config.dataProviders.should_be(new [] { "newDp1", "newDp2" });
                    config.codeGenerators.should_be(new [] { "newCg1", "newCg2" });
                    config.postProcessors.should_be(new [] { "newPp1", "newPp2" });
                };

                it["gets string"] = () => {
                    config.ToString().should_be(
                        "Entitas.CodeGeneration.CodeGenerator.SearchPaths = newS1, newS2\n" +
                        "Entitas.CodeGeneration.CodeGenerator.Plugins = newP1, newP2\n" +

                        "Entitas.CodeGeneration.CodeGenerator.DataProviders = newDp1, newDp2\n" +
                        "Entitas.CodeGeneration.CodeGenerator.CodeGenerators = newCg1, newCg2\n" +
                        "Entitas.CodeGeneration.CodeGenerator.PostProcessors = newPp1, newPp2\n"
                    );
                };
            };
        };
    }
}
