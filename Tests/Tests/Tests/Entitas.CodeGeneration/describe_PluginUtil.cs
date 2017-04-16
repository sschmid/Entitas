using NSpec;
using Entitas.CodeGeneration.Plugins;

class describe_PluginUtil : nspec {

    void when_plugin_util() {

        xit["gets same dependeny resolver for same assemblies"] = () => {
            var assemblies = new [] { "a1", "a2" };

            var r1 = PluginUtil.GetAssembliesResolver(assemblies, new string[0]);
            var r2 = PluginUtil.GetAssembliesResolver(assemblies, new string[0]);

            r1.should_be_same(r2);
        };

        xit["gets different dependeny resolver for different assemblies"] = () => {
            var assemblies1 = new [] { "a1", "a2" };
            var assemblies2 = new [] { "a2", "a3" };

            var r1 = PluginUtil.GetAssembliesResolver(assemblies1, new string[0]);
            var r2 = PluginUtil.GetAssembliesResolver(assemblies2, new string[0]);

            r1.should_not_be_same(r2);
        };
    }
}
