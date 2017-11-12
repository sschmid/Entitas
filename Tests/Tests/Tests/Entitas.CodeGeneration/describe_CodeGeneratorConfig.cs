using Entitas.CodeGeneration.CodeGenerator;
using Entitas.Utils;
using NSpec;

class describe_CodeGeneratorConfig : nspec {

    const string configString =
        "CodeGenerator.SearchPaths = sp1, sp2" + "\n" +
        "CodeGenerator.Plugins = p1, p2" + "\n" +

        "CodeGenerator.DataProviders = dp1,dp2,dp3" + "\n" +
        "CodeGenerator.CodeGenerators = cg1, cg2, cg3" + "\n" +
        "CodeGenerator.PostProcessors = pp1 , pp2 , pp3" + "\n";

    void when_creating_config() {

        CodeGeneratorConfig config = null;

        before = () => {
            config = new CodeGeneratorConfig();
        };

        context["when input string"] = () => {

            before = () => {
                config.Configure(new Preferences(new Properties(configString)));
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
                        "CodeGenerator.SearchPaths = newS1, \\\n" +
                        "                            newS2\n\n" +

                        "CodeGenerator.Plugins = newP1, \\\n" +
                        "                        newP2\n\n" +

                        "CodeGenerator.DataProviders = newDp1, \\\n" +
                        "                              newDp2\n\n" +

                        "CodeGenerator.CodeGenerators = newCg1, \\\n" +
                        "                               newCg2\n\n" +

                        "CodeGenerator.PostProcessors = newPp1, \\\n" +
                        "                               newPp2\n\n"
                    );
                };
            };
        };
    }
}
